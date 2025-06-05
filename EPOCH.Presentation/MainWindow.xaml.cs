using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using SGPdotNET.Observation;
using Rectangle = System.Drawing.Rectangle;
using OpenTkIntegrationTest;
using WpfPoint = System.Windows.Point;
using EPOCH.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using SGPdotNET.CoordinateSystem;

namespace EPOCH.Presentation
{
    public partial class MainWindow : Window
    {
        private const float Sensitivity = 0.3f;
        private const float ZoomStep = 1500f;
        private const float MinDistance = 8000f;
        private const float MaxDistance = 40000f;
        private const float EarthRadius = 6371.0f;

        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan PredictionInterval = TimeSpan.FromSeconds(0.01);

        private readonly string _earthObjPath = "../../../earth.obj";
        private readonly string _satObjPath = "../../../satellite.obj";
        private readonly string _earthTexturePath = "../../../2k_earth.png";

        private readonly Satellite _iss = new("ISS (ZARYA)",
            "1 25544U 98067A   25146.54650260  .00010397  00000+0  19155-3 0  9999",
            "2 25544  51.6382  54.2937 0002241 147.4648 271.6158 15.49752720511807");

        private readonly object _satelliteLock = new();

        private Dictionary<string, Vector3> _satellitePositions = new();
        private IEnumerable<Domain.Entities.Satellite> _allSatellites = Enumerable.Empty<Domain.Entities.Satellite>();
        private List<Satellite> _sgpSatellites = new();
        private readonly GetAllSatellitesUseCase _getSatellitesUseCase;

        private int _vao, _shaderProgram, _satVao, _shaderSatProgram;
        private uint[] _indices, _satIndices;
        private float _cameraDistance = 20000f;
        private float _yaw = 90f, _pitch = 0f, _scaleFactor = 1f;
        private bool _setupIsDone, _isDragging;
        private Vector2 _lastMousePos, _mouseDelta;
        private Vector3 _cameraPosition;

        private Matrix4 _view, _projection;

        public MainWindow()
        {
            InitializeComponent();

            _getSatellitesUseCase = ((App)System.Windows.Application.Current).ServiceProvider.GetRequiredService<GetAllSatellitesUseCase>();
            OpenTkControl.Start(new GLWpfControlSettings { MajorVersion = 3, MinorVersion = 3, RenderContinuously = true });

            AttachEventHandlers();

            Task.Run(LoadSatellites);
            Task.Run(UpdatePredictionsLoop);
        }

        private void AttachEventHandlers()
        {
            OpenTkControl.MouseMove += (s, e) =>
            {
                if (!_isDragging) return;
                var pos = e.GetPosition(OpenTkControl).ToVector2();
                _mouseDelta = pos - _lastMousePos;
                _lastMousePos = pos;
            };

            OpenTkControl.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _isDragging = true;
                    _lastMousePos = e.GetPosition(OpenTkControl).ToVector2();
                }
            };

            OpenTkControl.MouseUp += (s, e) => _isDragging = false;

            OpenTkControl.MouseWheel += (s, e) =>
            {
                _cameraDistance = Math.Clamp(_cameraDistance - Math.Sign(e.Delta) * ZoomStep, MinDistance, MaxDistance);
            };

            OpenTkControl.Render += (delta) => RenderFrame();
        }

        private async Task LoadSatellites()
        {
            await UpdateSatelliteCache();
            while (true)
            {
                await Task.Delay(UpdateInterval);
                await UpdateSatelliteCache();
            }
        }

        private async Task UpdateSatelliteCache()
        {
            try
            {
                var satellites = await _getSatellitesUseCase.ExecuteAsync();
                lock (_satelliteLock)
                {
                    _allSatellites = satellites;
                    _sgpSatellites = satellites.Select(s => new Satellite(s.Name, s.TleData.TleLine1, s.TleData.TleLine2)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Satellite update failed: {ex.Message}");
            }
        }

        private async Task UpdatePredictionsLoop()
        {
            while (true)
            {
                var newPositions = new Dictionary<string, Vector3>();
                var now = DateTime.UtcNow;

                lock (_satelliteLock)
                {
                    foreach (var sat in _sgpSatellites)
                    {
                        try
                        {
                            var geo = sat.Predict(now).ToGeodetic();
                            newPositions[sat.Name] = ToCartesian(geo);
                        }
                        catch { }
                    }
                }

                _satellitePositions = newPositions;
                await Task.Delay(PredictionInterval);
            }
        }

        private void RenderFrame()
        {
            if (!_setupIsDone) SetupOpenGL();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_isDragging)
            {
                _yaw += _mouseDelta.X * Sensitivity;
                _pitch = Math.Clamp(_pitch - _mouseDelta.Y * Sensitivity, -89f, 89f);
            }
            _mouseDelta = Vector2.Zero;

            var front = Vector3.Normalize(new Vector3(
                MathF.Cos(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(_pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch))));

            _cameraPosition = -front * _cameraDistance;
            _view = Matrix4.LookAt(_cameraPosition, Vector3.Zero, Vector3.UnitY);

            DrawObject(_shaderProgram, _vao, Matrix4.CreateScale(_scaleFactor), _indices);

            lock (_satelliteLock)
            {
                foreach (var pos in _satellitePositions.Values)
                {
                    DrawObject(_shaderSatProgram, _satVao, Matrix4.CreateTranslation(pos) * Matrix4.CreateScale(_scaleFactor), _satIndices);
                }
            }

            var issGeo = _iss.Predict(DateTime.UtcNow).ToGeodetic();
            var issPos = ToCartesian(issGeo);
            DrawObject(_shaderSatProgram, _satVao, Matrix4.CreateTranslation(issPos) * Matrix4.CreateScale(_scaleFactor), _satIndices);
        }

        private void SetupOpenGL()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0f, 0f, 0f, 1f);

            ObjLoader.Load(_earthObjPath, out var vertices, out _indices);
            ObjLoader.Load(_satObjPath, out var satVertices, out _satIndices);

            _vao = CreateVAO(vertices, _indices, true);
            _satVao = CreateVAO(satVertices, _satIndices, false);

            _shaderProgram = CompileShaders(ShaderSource.Vertex, ShaderSource.Fragment);
            _shaderSatProgram = CompileShaders(ShaderSource.VertexSat, ShaderSource.FragmentSat);

            LoadTexture(_earthTexturePath);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f),
                (float)(OpenTkControl.ActualWidth / OpenTkControl.ActualHeight), 0.1f, 70000.0f);

            _setupIsDone = true;
        }

        private void DrawObject(int shader, int vao, Matrix4 model, uint[] indexBuffer)
        {
            GL.UseProgram(shader);
            GL.BindVertexArray(vao);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "view"), false, ref _view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref _projection);
            GL.DrawElements(PrimitiveType.Triangles, indexBuffer.Length, DrawElementsType.UnsignedInt, 0);
        }

        private int CompileShaders(string vertexSource, string fragmentSource)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSource);
            GL.CompileShader(vertex);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSource);
            GL.CompileShader(fragment);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.LinkProgram(program);

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
            return program;
        }

        private int CreateVAO(float[] vertices, uint[] indices, bool hasTexCoords)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            if (hasTexCoords)
            {
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }

            return vao;
        }

        private Vector3 ToCartesian(GeodeticCoordinate geo) => new(
            (float)((EarthRadius + geo.Altitude) * Math.Cos(geo.Latitude.Radians) * Math.Sin(geo.Longitude.Radians)),
            (float)((EarthRadius + geo.Altitude) * Math.Sin(geo.Latitude.Radians)),
            (float)((EarthRadius + geo.Altitude) * Math.Cos(geo.Latitude.Radians) * Math.Cos(geo.Longitude.Radians)));

        private int LoadTexture(string path)
        {
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);

            using var image = new Bitmap(path);
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return tex;
        }
    }

    public static class ShaderSource
    {
        public const string Vertex = @"#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
out vec2 texCoord;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main() {
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
    texCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);
}";

        public const string Fragment = @"#version 330 core
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D texture0;
void main() {
    FragColor = texture(texture0, texCoord);
}";

        public const string VertexSat = @"#version 330 core
layout (location = 0) in vec3 aPosition;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main() {
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
}";

        public const string FragmentSat = @"#version 330 core
out vec4 FragColor;
void main() {
    FragColor = vec4(1.0,1.0,1.0,1.0);
}";
    }

    public static class Extensions
    {
        public static Vector2 ToVector2(this WpfPoint p) => new((float)p.X, (float)p.Y);
    }
}
