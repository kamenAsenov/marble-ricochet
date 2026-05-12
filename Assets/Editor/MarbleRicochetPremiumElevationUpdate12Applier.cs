#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetPremiumElevationUpdate12Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Premium Elevation Update 12")]
    public static void ApplyUpdate()
    {
        if(Application.isPlaying){ Debug.LogError("Stop Play Mode before applying Update 12."); return; }
        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        if(manager == null){ EditorUtility.DisplayDialog("Update 12", "Could not find RPGameManager. Open the Marble Ricochet scene first.", "OK"); return; }

        Material tableMat = CreateMaterial("MR12_Premium_Walnut_Table", new Color(0.23f,0.105f,0.042f), 0.62f);
        Material inlayMat = CreateEmissionMaterial("MR12_Subtle_Gold_Inlay", new Color(0.88f,0.54f,0.18f), 0.28f);
        Material railMat = CreateMaterial("MR12_Deep_Rubber_Rails", new Color(0.030f,0.027f,0.024f), 0.40f);
        Material railEdgeMat = CreateEmissionMaterial("MR12_Premium_Rail_Gold_Edge", new Color(1.0f,0.64f,0.22f), 0.55f);
        Material glassMat = CreateTransparentMaterial("MR12_Icy_Glass_Target", new Color(0.36f,0.88f,1.0f,0.62f), 0.98f);
        Material glassWallMat = CreateTransparentMaterial("MR12_Clear_Breakable_Glass", new Color(0.68f,0.95f,1.0f,0.44f), 0.94f);
        Material bumperMat = CreateEmissionMaterial("MR12_Brass_Bumper_Glow", new Color(1.0f,0.58f,0.18f), 0.72f);
        Material marbleMat = CreateMaterial("MR12_Premium_Marble_Ball", new Color(0.96f,0.94f,0.86f), 0.90f);
        Material shardMat = CreateEmissionMaterial("MR12_Glass_Spark_Shards", new Color(0.62f,0.95f,1.0f), 1.45f);

        ApplyMaterials(manager, railMat, glassMat, glassWallMat, bumperMat, marbleMat);
        CreatePremiumTableVisuals(tableMat, inlayMat, railMat, railEdgeMat);
        CreatePremiumLighting();
        CreatePremiumGlassEffect(manager, shardMat);
        CreatePremiumWinPolish(manager);
        CreateFirstFivePolishHint(manager);
        PolishShopCopy();
        PolishWinLayout(manager);
        SaveScene();
        EditorUtility.DisplayDialog("Update 12 Applied", "Premium Elevation Update 12 applied. Visuals, lighting, glass feedback, win flow and shop polish were improved.", "OK");
    }

    private static void ApplyMaterials(RPGameManager manager, Material railMat, Material glassMat, Material glassWallMat, Material bumperMat, Material marbleMat)
    {
        manager.wallMaterial = railMat; manager.targetMaterial = glassMat; manager.breakableGlassMaterial = glassWallMat; manager.metalBumperMaterial = bumperMat; manager.ballMaterial = marbleMat;
        ApplyRenderer(manager.wallPrefab, railMat); ApplyRenderer(manager.targetPrefab, glassMat); ApplyRenderer(manager.breakableGlassPrefab, glassWallMat); ApplyRenderer(manager.metalBumperPrefab, bumperMat); if(manager.ball != null) ApplyRenderer(manager.ball.gameObject, marbleMat);
    }
    private static void ApplyRenderer(GameObject obj, Material mat){ if(obj==null || mat==null) return; MeshRenderer r = obj.GetComponent<MeshRenderer>(); if(r!=null){ r.sharedMaterial = mat; EditorUtility.SetDirty(r); } }

    private static void CreatePremiumTableVisuals(Material tableMat, Material inlayMat, Material railMat, Material railEdgeMat)
    {
        GameObject old = GameObject.Find("MR12_Premium_Table_Visuals"); if(old != null) Object.DestroyImmediate(old);
        GameObject root = new GameObject("MR12_Premium_Table_Visuals");
        CreateCube(root.transform,"Premium_Walnut_Surface",new Vector3(0f,-0.005f,0f),new Vector3(11.55f,0.045f,15.55f),tableMat).AddComponent<RPPremiumAmbientMotion>().pulseAmount = 0.002f;
        for(int i=-5;i<=5;i++) CreateCube(root.transform,"Fine_Gold_Inlay_Long_"+i,new Vector3(i*0.95f,0.045f,0f),new Vector3(0.020f,0.018f,14.55f),inlayMat);
        for(int z=-6;z<=6;z+=3) CreateCube(root.transform,"Fine_Gold_Inlay_Cross_"+z,new Vector3(0f,0.047f,z),new Vector3(10.4f,0.016f,0.020f),inlayMat);
        CreateCube(root.transform,"Premium_Top_Rail",new Vector3(0f,0.23f,7.12f),new Vector3(10.95f,0.44f,0.42f),railMat);
        CreateCube(root.transform,"Premium_Bottom_Rail",new Vector3(0f,0.23f,-7.12f),new Vector3(10.95f,0.44f,0.42f),railMat);
        CreateCube(root.transform,"Premium_Left_Rail",new Vector3(-5.12f,0.23f,0f),new Vector3(0.42f,0.44f,14.25f),railMat);
        CreateCube(root.transform,"Premium_Right_Rail",new Vector3(5.12f,0.23f,0f),new Vector3(0.42f,0.44f,14.25f),railMat);
        CreateCube(root.transform,"Premium_Top_Rail_Gold_Lip",new Vector3(0f,0.49f,6.78f),new Vector3(10.25f,0.055f,0.075f),railEdgeMat);
        CreateCube(root.transform,"Premium_Bottom_Rail_Gold_Lip",new Vector3(0f,0.49f,-6.78f),new Vector3(10.25f,0.055f,0.075f),railEdgeMat);
        CreateCube(root.transform,"Premium_Left_Rail_Gold_Lip",new Vector3(-4.78f,0.49f,0f),new Vector3(0.075f,0.055f,13.45f),railEdgeMat);
        CreateCube(root.transform,"Premium_Right_Rail_Gold_Lip",new Vector3(4.78f,0.49f,0f),new Vector3(0.075f,0.055f,13.45f),railEdgeMat);
        CreateCorner(root.transform,new Vector3(-5.12f,0.54f,-7.12f),railEdgeMat); CreateCorner(root.transform,new Vector3(5.12f,0.54f,-7.12f),railEdgeMat); CreateCorner(root.transform,new Vector3(-5.12f,0.54f,7.12f),railEdgeMat); CreateCorner(root.transform,new Vector3(5.12f,0.54f,7.12f),railEdgeMat);
    }
    private static GameObject CreateCube(Transform parent,string name,Vector3 pos,Vector3 scale,Material mat){ GameObject o=GameObject.CreatePrimitive(PrimitiveType.Cube); o.name=name; o.transform.SetParent(parent); o.transform.position=pos; o.transform.localScale=scale; MeshRenderer r=o.GetComponent<MeshRenderer>(); if(r!=null) r.sharedMaterial=mat; Collider c=o.GetComponent<Collider>(); if(c!=null) Object.DestroyImmediate(c); return o; }
    private static void CreateCorner(Transform parent, Vector3 pos, Material mat){ GameObject o=GameObject.CreatePrimitive(PrimitiveType.Sphere); o.name="Premium_Rail_Corner"; o.transform.SetParent(parent); o.transform.position=pos; o.transform.localScale=new Vector3(0.26f,0.26f,0.26f); MeshRenderer r=o.GetComponent<MeshRenderer>(); if(r!=null) r.sharedMaterial=mat; Collider c=o.GetComponent<Collider>(); if(c!=null) Object.DestroyImmediate(c); }

    private static void CreatePremiumLighting()
    {
        GameObject old=GameObject.Find("MR12_Premium_Lighting"); if(old!=null) Object.DestroyImmediate(old); GameObject root=new GameObject("MR12_Premium_Lighting");
        CreateLight(root.transform,"Warm_Table_Key",LightType.Directional,new Vector3(0f,8f,-3f),Quaternion.Euler(62f,0f,24f),new Color(1f,0.82f,0.58f),1.85f,10f);
        CreateLight(root.transform,"Cool_Glass_Fill",LightType.Point,new Vector3(0f,4.2f,1.5f),Quaternion.identity,new Color(0.45f,0.70f,1f),1.10f,11f);
        CreateLight(root.transform,"Gold_Rim_Accent",LightType.Point,new Vector3(0f,3.6f,-5.3f),Quaternion.identity,new Color(1f,0.62f,0.22f),0.75f,7f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight = new Color(0.27f,0.22f,0.17f);
    }
    private static void CreateLight(Transform p,string n,LightType t,Vector3 pos,Quaternion rot,Color col,float intensity,float range){ GameObject o=new GameObject(n); o.transform.SetParent(p); o.transform.position=pos; o.transform.rotation=rot; Light l=o.AddComponent<Light>(); l.type=t; l.color=col; l.intensity=intensity; l.range=range; }

    private static void CreatePremiumGlassEffect(RPGameManager manager, Material shardMat)
    {
        GameObject old=GameObject.Find("MR12_PremiumGlassPopEffectPrefab"); if(old!=null) Object.DestroyImmediate(old); GameObject effect=new GameObject("MR12_PremiumGlassPopEffectPrefab"); effect.transform.position=new Vector3(100f,100f,100f); effect.SetActive(false);
        ParticleSystem ps=effect.AddComponent<ParticleSystem>(); var main=ps.main; main.duration=0.28f; main.loop=false; main.startLifetime=0.62f; main.startSpeed=3.8f; main.startSize=0.075f; main.maxParticles=90;
        var em=ps.emission; em.rateOverTime=0f; em.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0f,42), new ParticleSystem.Burst(0.055f,16) });
        var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Sphere; shape.radius=0.34f;
        var vel=ps.velocityOverLifetime; vel.enabled=true; vel.space=ParticleSystemSimulationSpace.Local; vel.y=new ParticleSystem.MinMaxCurve(0.2f,0.8f);
        ParticleSystemRenderer renderer=effect.GetComponent<ParticleSystemRenderer>(); if(renderer!=null) renderer.sharedMaterial=shardMat;
        RPEffectAutoDestroy autoDestroy=effect.AddComponent<RPEffectAutoDestroy>(); autoDestroy.lifetime=1.35f;
        if(manager.targetPrefab!=null){ RPTarget target=manager.targetPrefab.GetComponent<RPTarget>(); if(target!=null){ target.popEffectPrefab=effect; EditorUtility.SetDirty(target); } }
        if(manager.breakableGlassPrefab!=null){ RPObstacle obstacle=manager.breakableGlassPrefab.GetComponent<RPObstacle>(); if(obstacle!=null){ obstacle.breakEffectPrefab=effect; EditorUtility.SetDirty(obstacle); } }
    }

    private static void CreatePremiumWinPolish(RPGameManager manager){ RPPremiumWinPolish p=Object.FindFirstObjectByType<RPPremiumWinPolish>(); if(p==null){ GameObject o=new GameObject("RPPremiumWinPolish"); p=o.AddComponent<RPPremiumWinPolish>(); } p.gameManager=manager; p.winPanel=manager.winPanel; p.rewardText=manager.winRewardText; p.titleText=manager.winTitleText; EditorUtility.SetDirty(p); }
    private static void CreateFirstFivePolishHint(RPGameManager manager){ RPFirstFivePolishHint h=Object.FindFirstObjectByType<RPFirstFivePolishHint>(); if(h==null){ GameObject o=new GameObject("RPFirstFivePolishHint"); h=o.AddComponent<RPFirstFivePolishHint>(); } h.gameManager=manager; EditorUtility.SetDirty(h); }
    private static void PolishShopCopy(){ SetText("AdStatusText",16,"Optional rewards. No forced ads in test build."); }
    private static void PolishWinLayout(RPGameManager manager){ if(manager.winTitleText!=null){ manager.winTitleText.fontSize=36; manager.winTitleText.color=new Color(1f,0.78f,0.34f); EditorUtility.SetDirty(manager.winTitleText); } if(manager.winRewardText!=null){ manager.winRewardText.fontSize=20; manager.winRewardText.color=new Color(1f,0.76f,0.36f); EditorUtility.SetDirty(manager.winRewardText); } Text next=GameObject.Find("NextLevelPreviewText")?.GetComponent<Text>(); if(next!=null){ next.fontSize=16; next.color=new Color(0.75f,0.90f,1f); EditorUtility.SetDirty(next); } }
    private static void SetText(string name,int size,string textIfEmpty){ Text t=GameObject.Find(name)?.GetComponent<Text>(); if(t==null) return; t.fontSize=size; if(string.IsNullOrWhiteSpace(t.text)) t.text=textIfEmpty; EditorUtility.SetDirty(t); }

    private static Material CreateMaterial(string name, Color color, float smoothness){ string folderPath="Assets/Materials"; if(!AssetDatabase.IsValidFolder(folderPath)) AssetDatabase.CreateFolder("Assets","Materials"); string assetPath=folderPath+"/"+name+".mat"; Material existing=AssetDatabase.LoadAssetAtPath<Material>(assetPath); Shader shader=Shader.Find("Universal Render Pipeline/Lit"); if(shader==null) shader=Shader.Find("Universal Render Pipeline/Simple Lit"); if(shader==null) shader=Shader.Find("Standard"); Material mat=existing!=null?existing:new Material(shader); mat.shader=shader; if(mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor",color); mat.color=color; if(mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness",smoothness); if(existing==null) AssetDatabase.CreateAsset(mat,assetPath); EditorUtility.SetDirty(mat); AssetDatabase.SaveAssets(); return mat; }
    private static Material CreateTransparentMaterial(string name, Color color, float smoothness){ Material mat=CreateMaterial(name,color,smoothness); if(mat.HasProperty("_Surface")) mat.SetFloat("_Surface",1f); if(mat.HasProperty("_Blend")) mat.SetFloat("_Blend",0f); if(mat.HasProperty("_AlphaClip")) mat.SetFloat("_AlphaClip",0f); if(mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor",color); mat.color=color; mat.renderQueue=3000; EditorUtility.SetDirty(mat); AssetDatabase.SaveAssets(); return mat; }
    private static Material CreateEmissionMaterial(string name, Color color, float emissionStrength){ Material mat=CreateMaterial(name,color,0.60f); if(mat.HasProperty("_EmissionColor")){ mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",color*emissionStrength); } EditorUtility.SetDirty(mat); AssetDatabase.SaveAssets(); return mat; }
    private static void SaveScene(){ Scene s=SceneManager.GetActiveScene(); if(s.IsValid()){ EditorSceneManager.MarkSceneDirty(s); EditorSceneManager.SaveOpenScenes(); } AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
}
#endif