/**************************
 * 文件名:UZipDemo.cs;
 * 文件描述:UZipDemo;
 * 创建日期:2016/05/25;
 * Author:Create by ThisisGame;
 * Page:https://github.com/ThisisGame/UZip
 ***************************/

using UnityEngine;
using UnityEngine.UI;

namespace ThisisGame.UZip
{
    public class UZipDemo : MonoBehaviour
    {
        [SerializeField]
        Slider slider;


        UZip uZip = new UZip();

        void Update()
        {
            if(this.uZip!=null)
            {
                this.uZip.Update();
            }
        }

        void OnGUI()
        {
            if (GUILayout.Button("Compress Halo Folder"))
            {
                slider.value = 0;

                this.uZip.Compress( Application.dataPath + "/UZip/Tutorials/Halo", "Halo.zip", "", (srcPath, targetPath, processfile, percent) => 
                {
                    Debug.Log("Process srcPath=" + srcPath);
                    Debug.Log("Process targetPath=" + targetPath);
                    Debug.Log("Process processfile=" + processfile);
                    Debug.Log("Process percent=" + percent);

                    slider.value = percent;
                }, 
                (srcPath, targetPath) => 
                {
                    Debug.Log("Complete srcPath=" + srcPath);
                    Debug.Log("Complete targetPath=" + targetPath);

                    slider.value = 1;
                }, 
                (srcPath, targetPath,exception) =>
                {
                    Debug.LogError("Error srcPath=" + srcPath);
                    Debug.LogError("Error targetPath=" + targetPath);
                    Debug.LogError("Error exception=" + exception);
                });
            }

            if (GUILayout.Button("DeCompress Halo.zip"))
            {
                slider.value = 0;

                this.uZip.DeCompress("Halo.zip", "Halo", "", (srcPath, targetPath, processfile, percent) =>
                {
                    Debug.Log("Process srcPath=" + srcPath);
                    Debug.Log("Process targetPath=" + targetPath);
                    Debug.Log("Process processfile=" + processfile);
                    Debug.Log("Process percent=" + percent);

                    slider.value = percent;
                },
                (srcPath, targetPath) =>
                {
                    Debug.Log("Complete srcPath=" + srcPath);
                    Debug.Log("Complete targetPath=" + targetPath);

                    slider.value = 1;
                },
                (srcPath, targetPath, exception) =>
                {
                    Debug.LogError("Error srcPath=" + srcPath);
                    Debug.LogError("Error targetPath=" + targetPath);
                    Debug.LogError("Error exception=" + exception);
                });
            }
        }
    }
}

