using Colorful.RenderGraph;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "BRenderPipeline/BRenderPieplineAsset")]
public class BRenderPipelineAsset : RenderPipelineAsset
{
    public RenderGraphData renderGraph;
    public Texture SkyBoxTex;
    protected override RenderPipeline CreatePipeline()
    {

        return new BRenderPipeline(this);
    }
}

public class BRenderPipeline : RenderPipeline
{
    RenderGraphData mRenderGraph;

    BRenderPipelineAsset mAsset;
    public BRenderPipeline(BRenderPipelineAsset bRenderPipelineAsset)
    {
        mRenderGraph = bRenderPipelineAsset.renderGraph;
        mAsset = bRenderPipelineAsset;
    }
    protected override void Dispose(bool disposing)
    {

        base.Dispose(disposing);
        if (mRenderGraph != null)
            mRenderGraph.Cleanup();
        mRenderGraph = null;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        if (mRenderGraph == null)
            return;
        BeginFrameRendering(context, cameras);
        foreach (var camera in cameras)
        {
            BeginCameraRendering(context, camera);

            //mRenderGraph.ExcEute(context, camera);
            mRenderGraph.Excute(context, camera);
            context.Submit();
            EndCameraRendering(context, camera);
        }
        mRenderGraph.EndFrame();
        EndFrameRendering(context, cameras);
    }
}
