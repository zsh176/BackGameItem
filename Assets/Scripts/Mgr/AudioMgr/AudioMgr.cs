using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Ч����ģ��
/// </summary>
public class AudioMgr : InstanceMgr<AudioMgr>
{
    //Ψһ�ı����������
    private AudioSource bkMusic = null;
    //���ִ�С
    private float bkValue = 1;

    //��Ч��������
    private GameObject soundObj = null;
    //��Ч�б�
    private List<AudioSource> soundList = new List<AudioSource>();
    //��Ч��С
    private float soundValue = 1;

    public AudioMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    private void Update()
    {
        if (soundObj == null) return;
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            //��Ч������ͣ��ֹͣ��û�б�����״̬���Ƴ������Ч
            if (soundList[i] != null && !soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ���ű������� �� �л���������
    /// </summary>
    /// <param name="name">��������Դ��</param>
    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BkMusic";
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //�첽���ر������� ������ɺ� ����
        AddressablesMgr.Instance.LoadAssetAsync<AudioClip>(clip =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        }, name, "Clip");
    }

    /// <summary>
    /// ���ñ������ֿ���
    /// </summary>
    public void MusicSwitch(bool bol)
    {
        if (bkMusic != null)
        {
            bkMusic.mute = !bol;
        }
    }

    /// <summary>
    /// ��ͣ��������
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// �ı䱳������ ������С
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    public void PlaySound(string name, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
        }
        //����Ч��Դ�첽���ؽ����� �����һ����Ч
        AddressablesMgr.Instance.LoadAssetAsync<AudioClip>(clip =>
        {
            if (soundObj == null) return;
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            soundList.Add(source);
            if (callBack != null)
                callBack(source);
        }, name,"Clip");
    }

    /// <summary>
    /// �ı���Ч������С
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        if (soundObj == null) return;
        for (int i = 0; i < soundList.Count; ++i)
            soundList[i].volume = value;
    }

    /// <summary>
    /// ������Ч����
    /// </summary>
    public void SoundSwitch(bool bol)
    {
        if (soundObj == null) return;
        for (int i = 0; i < soundList.Count; ++i)
            soundList[i].mute = !bol;
    }

    /// <summary>
    /// ֹͣ��Ч
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (soundObj == null) return;
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
