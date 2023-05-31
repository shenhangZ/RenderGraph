using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;


public class GBufferPass : IRenderPass<GBufferPass>
{
    Material skyBox;
    ShaderTagId passId;

    [PortPin(PinType.Write)]
    public TextureHandle depthBuffer;
    [PortPin(PinType.Write)]
    public TextureHandle sceneColorDefferred; // lit from environment
    [PortPin(PinType.Write)]
    public TextureHandle gBufferPosWR;
    [PortPin(PinType.Write)]
    public TextureHandle gBufferNorWM;
    [PortPin(PinType.Write)]
    public TextureHandle gBufferAlbedo;
    [PortPin(PinType.Write)]
    public RendererListHandle rendererList;

    public enum E
    {
        a, b, c, d
    }

    public float a = 1;
    public bool b = false;
    public Vector2 c = new Vector2(1.10973f, 2.0f);
    public E d = E.d;
    public LayerMask e = 10;
    public Color f = new Color(1, 2, 3, 0.22334f);

    private Matrix4x4 view, proj;
    protected override void Execute(
        RenderGraphContext ctx)
    {
        var cmd = ctx.cmd;
        cmd.SetRenderTarget(new RenderTargetIdentifier[] {
                            sceneColorDefferred,
                            gBufferPosWR,
                            gBufferNorWM,
                            gBufferAlbedo }, depthBuffer);
        cmd.ClearRenderTarget(true, true, Color.black);

        cmd.SetViewProjectionMatrices(view, proj);
        cmd.DrawProcedural(Matrix4x4.identity, skyBox, 0, MeshTopology.Triangles, 36);

        cmd.DrawRendererList(rendererList);
    }

    protected override void AllocateWriteResource(Camera camera, ScriptableRenderContext context)
    {
        skyBox = new Material(Shader.Find("BRPipeline/Skybox"));
        passId = new ShaderTagId(nameof(GBufferPass));

        view = camera.worldToCameraMatrix;
        proj = camera.projectionMatrix;
        gBufferPosWR = CreateTexture
            (new TextureDesc(camera.pixelWidth, camera.pixelHeight)
            {
                colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat,
                name = "gBufferPosWR",
                clearBuffer = true,
                clearColor = Color.clear
            });
        sceneColorDefferred = CreateTexture(new TextureDesc(camera.pixelWidth, camera.pixelHeight)
        {
            colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat,
            name = "sceneColorDefferred",
            clearBuffer = true,
            clearColor = Color.black
        });
        gBufferNorWM = CreateTexture(new TextureDesc(camera.pixelWidth, camera.pixelHeight)
        {
            colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat,
            name = "gBufferNorWM",
            clearBuffer = true,
            clearColor = Color.clear
        });
        gBufferAlbedo = CreateTexture(new TextureDesc(camera.pixelWidth, camera.pixelHeight)
        {
            colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat,
            name = "gBufferAlbedo",
            clearBuffer = true,
            clearColor = Color.clear
        });
        depthBuffer = CreateTexture(new TextureDesc(camera.pixelWidth, camera.pixelHeight)
        {
            depthBufferBits = DepthBits.Depth32,
            name = "depthBuffer",
            clearBuffer = true,
        });

        camera.TryGetCullingParameters(out var cullingParameters);
        var cullResults = context.Cull(ref cullingParameters);
        UnityEngine.Rendering.RendererUtils.RendererListDesc gbuffer = new UnityEngine.Rendering.RendererUtils.RendererListDesc(passId, cullResults, camera);
        gbuffer.renderQueueRange = RenderQueueRange.all;
        gbuffer.sortingCriteria = SortingCriteria.None;
        rendererList = CreateRendererList(gbuffer);
    }
}
