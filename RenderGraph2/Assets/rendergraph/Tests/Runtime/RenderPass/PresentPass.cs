using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public class PresentPass : IRenderPass<PresentPass>
{
    [PortPin(PinType.Read)]
    public TextureHandle src;

    [PortPin(PinType.Write)]
    public TextureHandle bgColor;

    protected override void Execute(RenderGraphContext ctx)
    {
        var cmd = ctx.cmd;
        //ctx.cmd.Blit(PresentPassData.src, presentData.bgColor, new Vector2(1, -1), new Vector2(0, 1));
        cmd.Blit(src, bgColor);
    }

    protected override void AllocateWriteResource(Camera camera, ScriptableRenderContext context)
    {
        bgColor = ImportBackbuffer(camera.targetTexture);
    }
}

