using System.Text;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;


namespace Latte.Test;


class Program
{
    private static unsafe void Main()
    {
        var glfw = Glfw.GetApi();


        glfw.Init();

        glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
        glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);

        var window = glfw.CreateWindow(16 * 60, 9 * 60, "OpenGL Testing", null, null);
        glfw.MakeContextCurrent(window);



        var gl = GL.GetApi(glfw.GetProcAddress);

        gl.Viewport(0, 0, 16 * 60, 9 * 60);




        var vertexShaderSource = File.ReadAllText("Resources/vertex.glsl");
        var fragmentShaderSource = File.ReadAllText("Resources/fragment.glsl");

        var vertexShader = gl.CreateShader(ShaderType.VertexShader);
        var fragmentShader = gl.CreateShader(ShaderType.FragmentShader);


        gl.ShaderSource(vertexShader, vertexShaderSource);
        gl.ShaderSource(fragmentShader, fragmentShaderSource);

        gl.CompileShader(vertexShader);
        gl.CompileShader(fragmentShader);


        var shaderProgram = gl.CreateProgram();

        gl.AttachShader(shaderProgram, vertexShader);
        gl.AttachShader(shaderProgram, fragmentShader);
        gl.LinkProgram(shaderProgram);


        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader);




        var vertices = new float[]
        {
            -0.5f,  0.5f, 0.0f, // top left
             0.5f,  0.5f, 0.0f, // top right
             0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f // bottom left
        };

        var indices = new uint[]
        {
            0, 1, 2,
            0, 3, 2
        };


        var VAO = gl.GenVertexArray();
        var VBO = gl.GenBuffer();
        var EBO = gl.GenBuffer();

        gl.BindVertexArray(VAO);


        gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);

        fixed (float* verticesPtr = vertices)
            gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), verticesPtr, BufferUsageARB.StaticDraw);

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);

        fixed (uint* indicesPtr = indices)
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(indices.Length * sizeof(uint)), indicesPtr, BufferUsageARB.StaticDraw);


        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
        gl.EnableVertexAttribArray(0);




        gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);

        while (!glfw.WindowShouldClose(window))
        {
            gl.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            gl.Clear(ClearBufferMask.ColorBufferBit);

            gl.UseProgram(shaderProgram);
            gl.BindVertexArray(VAO);

            gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);


            glfw.SwapBuffers(window);
            glfw.PollEvents();
        }




        glfw.DestroyWindow(window);

        glfw.Terminate();
    }
}
