using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public class LoginPanel : BasePanel
{
    protected override void Awake()
    {
        //һ�������� ��Ϊ��Ҫִ�и����awake����ʼ��һЩ��Ϣ �����ҿؼ� ���¼�����
        base.Awake();
        //�����洦���Լ���һЩ��ʼ���߼�
    }

    // Use this for initialization
    void Start()
    {

        //GetControl<Button>("btnStart").onClick.AddListener(ClickStart);
        //GetControl<Button>("btnQuit").onClick.AddListener(ClickQuit);

        UIManager.AddCustomEventListener(GetControl<Button>("btnStart"), EventTriggerType.PointerEnter, (data) => {
            Debug.Log("����" );
        });
        UIManager.AddCustomEventListener(GetControl<Button>("btnStart"), EventTriggerType.PointerExit, (data) => {
            Debug.Log("�뿪");
        });
    }

    private void Drag(BaseEventData data)
    {

    }

    private void PointerDown(BaseEventData data)
    {

    }


    public override void ShowMe()
    {
        base.ShowMe();
        //��ʾ���ʱ ��Ҫִ�е��߼� ������� ��UI�������� ���Զ������ǵ���
        //ֻҪ��д����  �ͻ�ִ��������߼�
    }

    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnStart":
                Debug.Log("btnStart�����");
                break;
            case "btnQuit":
                Debug.Log("btnQuit�����");
                break;
        }
    }

    protected override void OnValueChangedToggle(string toggleName, bool value)
    {
        //���������������ж� ��������һ����ѡ����߶�ѡ��״̬�仯�� ��ǰ״̬���Ǵ����value
    }


    public void InitInfo()
    {
        Debug.Log("��ʼ������");
    }

    //�����ʼ��ť�Ĵ���
    public void ClickStart()
    {

    }

    //�����ʼ��ť�Ĵ���
    public void ClickQuit()
    {
        Debug.Log("S");
    }
}
