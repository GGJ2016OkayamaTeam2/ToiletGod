using UnityEngine;
using System;
using System.Collections;
using live2d;
using live2d.framework;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SimpleModel : MonoBehaviour
{
    public TextAsset mocFile;
    public TextAsset physicsFile;
    public Texture2D[] textureFiles;
	public TextAsset[] mtnFiles; // モーションファイル.
	public Boolean isMotionLoop = false;

    private Live2DModelUnity live2DModel;
    private EyeBlinkMotion eyeBlink = new EyeBlinkMotion();
    private L2DTargetPoint dragMgr = new L2DTargetPoint();
    private L2DPhysics physics;
    private Matrix4x4 live2DCanvasPos;

	private Live2DMotion motionAppeal;
	private MotionQueueManager motionManager;

    void Start()
    {
        Live2D.init();

        load();


    }


    void load()
    {
        live2DModel = Live2DModelUnity.loadModel(mocFile.bytes);

		// モーションのインスタンスの作成（mtnの読み込み）と設定
		motionAppeal = Live2DMotion.loadMotion( mtnFiles[ 0 ].bytes );
		motionAppeal.setFadeOut (5000);
		motionAppeal.setLoop( this.isMotionLoop );

		motionManager = new MotionQueueManager();//モーション管理クラスの作成.
		//play
		motionManager.startMotion(motionAppeal,true);

        for (int i = 0; i < textureFiles.Length; i++)
        {
            live2DModel.setTexture(i, textureFiles[i]);
        }

        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50.0f, 50.0f);

        if (physicsFile != null) physics = L2DPhysics.load(physicsFile.bytes);
    }


    void Update()
    {
        if (live2DModel == null) load();
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);
        if (!Application.isPlaying)
        {
            live2DModel.update();
			//motionManager.updateParam (live2DModel);
            return;
        }

		/*
        var pos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            //
        }
        else if (Input.GetMouseButton(0))
        {
            dragMgr.Set(pos.x / Screen.width * 2 - 1, pos.y / Screen.height * 2 - 1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragMgr.Set(0, 0);
        }
        */


        dragMgr.update();
        live2DModel.setParamFloat("PARAM_ANGLE_X", dragMgr.getX() * 30);
        live2DModel.setParamFloat("PARAM_ANGLE_Y", dragMgr.getY() * 30);

        live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", dragMgr.getX() * 10);

        live2DModel.setParamFloat("PARAM_EYE_BALL_X", -dragMgr.getX());
        live2DModel.setParamFloat("PARAM_EYE_BALL_Y", -dragMgr.getY());

        double timeSec = UtSystem.getUserTimeMSec() / 1000.0;
        double t = timeSec * 2 * Math.PI;
        live2DModel.setParamFloat("PARAM_BREATH", (float)(0.5f + 0.5f * Math.Sin(t / 3.0)));

        eyeBlink.setParam(live2DModel);

        if (physics != null) physics.updateParam(live2DModel);

		if (motionManager.isFinished ()) {
			StartCoroutine (FadeModel(0, 1));
		}
		live2DModel.update();
		motionManager.updateParam (live2DModel);
    }


    void OnRenderObject()
    {
        if (live2DModel == null) load();
        if (live2DModel.getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH_NOW) live2DModel.draw();
    }

	public IEnumerator FadeModel(float destOps, float time) {
		Debug.Log ("called");
		yield return new WaitForEndOfFrame ();
		for (int i = 0; i < textureFiles.Length; i++) {
			Debug.Log ("inside loop");
			live2DModel.setPartsOpacity(i, destOps);
		}
		/*
		List<float> ops = new List<float> ();
		for (int i = 0; i < textureFiles.Length; i++) {
			ops.Add(live2DModel.getPartsOpacity(i));
		}
		var startTime = Time.time;
		while (Time.time - startTime < time) {
			var rate = (Time.time - startTime) / time;
			var curOps = 0f;
			for (int i = 0; i < textureFiles.Length; i++) {
				curOps = destOps * rate + live2DModel.getPartsOpacity(i) * (1 - rate);
				live2DModel.setPartsOpacity(i, curOps);
			}
			yield return new WaitForEndOfFrame ();
		}
		for (int i = 0; i < textureFiles.Length; i++) {
			live2DModel.setPartsOpacity(i, destOps);
		}
		*/
	}
}