using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//��������ռ�

public class EventCenter : MonoBehaviour
{
    #region �¼��㲥����
    //�˴��Ĵ�����Բ����޸�ֱ��ʹ��
    //�����ֵ䣬�����µ�ί���б�
    private static Dictionary<EventType, Delegate> EventTable = new Dictionary<EventType, Delegate>();

    ///<summary>
    ///��Ӽ���
    ///�������Ϊ��Ӻ���
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>

    private static void OnListenerAdding(EventType eventType, Delegate callBack)
    {
        if (!EventTable.ContainsKey(eventType))//�ж��Ƿ����¼�����
        {
            //���û�������
            EventTable.Add(eventType, null);
        }

        Delegate d = EventTable[eventType];

        if (d != null && d.GetType() != callBack.GetType())//d��Ϊ��ֵ����d�����Ͳ�Ϊ��ȡ��������
        {
            //����
            throw new Exception(string.Format("����Ϊ�¼�{0}��Ӳ�ͬ���͵�ί�У���ǰ�¼���ί����{1},Ҫ��Ӷ����¼�ί������Ϊ{2}", eventType, d.GetType(), callBack.GetType()));
            //��������Ƿ�������ȷ������������
        }
    }

    ///<summary>
    ///�Ƴ�����
    ///�������Ϊɾ������
    /// </summary>
    ///<param name="eventType"></param>
    ///<param name="callBack"></param>

    private static void OnListenerRemoving(EventType eventType, Delegate callBack)
    {
        if (EventTable.ContainsKey(eventType))
        {
            Delegate d = EventTable[eventType];
            if (d == null)
            {
                //û�ж�Ӧ�źţ����ߺ��뱻�Ƴ�
                throw new Exception(string.Format("�Ƴ����������¼�{0}û�ж�Ӧ��ί��", eventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                //��������Ƿ�������ȷ������������
                throw new Exception(string.Format("�Ƴ��������󣺳���Ϊ�¼�{0}�Ƴ���ͬ���͵�ί�У���ǰ��ί������Ϊ{1},Ҫ�Ƴ���ί������Ϊ{2}", eventType, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            //û���¼���
            throw new Exception(string.Format("�Ƴ���������û���¼���{0}", eventType));
        }
    }

    private static void OnListenerRemoved(EventType eventType)
    {
        if (EventTable[eventType] == null)//������뱻ɾ�� 
        {
            EventTable.Remove(eventType);
        }
    }
    #endregion

    #region 0�Ź㲥���ͣ��ɸ����޸ģ�
    public static void AddListener(EventType eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);//�ڻ�վ�����
        EventTable[eventType] = (CallBack)EventTable[eventType] + callBack;//���ֻ�����Ӻ��룬+xxx,xxx
    }

    public static void RemoveListener(EventType eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);//�ڻ�վ��ɾ��
        EventTable[eventType] = (CallBack)EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);//��ֹͨѶ
    }

    public static void Broadcast(EventType eventType)
    {
        Delegate d;
        if (EventTable.TryGetValue(eventType, out d))
        {
            CallBack callBack = d as CallBack;
            if (callBack != null)
            {
                callBack();//����
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    #endregion

    #region 1�Ź㲥���ͣ��ɸ����޸ģ�
    public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);//�ڻ�վ�����
        EventTable[eventType] = (CallBack<T>)EventTable[eventType] + callBack;//���ֻ�����Ӻ��룬+xxx,xxx
    }

    public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);//�ڻ�վ��ɾ��
        EventTable[eventType] = (CallBack<T>)EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);//��ֹͨѶ
    }

    public static void Broadcast<T>(EventType eventType, T arg)
    {
        Delegate d;
        if (EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T> callBack = d as CallBack<T>;
            if (callBack != null)
            {
                callBack(arg);//����
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    #endregion

    #region 2�Ź㲥���ͣ��ɸ����޸ģ�
    public static void AddListener<T, X>(EventType eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);//�ڻ�վ�����
        EventTable[eventType] = (CallBack<T, X>)EventTable[eventType] + callBack;//���ֻ�����Ӻ��룬+xxx,xxx
    }

    public static void RemoveListener<T, X>(EventType eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);//�ڻ�վ��ɾ��
        EventTable[eventType] = (CallBack<T, X>)EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);//��ֹͨѶ
    }

    public static void Broadcast<T, X>(EventType eventType, T arg1, X arg2)
    {
        Delegate d;
        if (EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X> callBack = d as CallBack<T, X>;
            if (callBack != null)
            {
                callBack(arg1, arg2);//����
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    #endregion

    #region 3�Ź㲥���ͣ��ɸ����޸ģ�
    public static void AddListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);//�ڻ�վ�����
        EventTable[eventType] = (CallBack<T, X, Y>)EventTable[eventType] + callBack;//���ֻ�����Ӻ��룬+xxx,xxx
    }

    public static void RemoveListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);//�ڻ�վ��ɾ��
        EventTable[eventType] = (CallBack<T, X, Y>)EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);//��ֹͨѶ
    }

    public static void Broadcast<T, X, Y>(EventType eventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        if (EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);//����
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    #endregion

    #region 3�Ź㲥���ͣ��ɸ����޸ģ�
    public static void AddListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);//�ڻ�վ�����
        EventTable[eventType] = (CallBack<T, X, Y, Z>)EventTable[eventType] + callBack;//���ֻ�����Ӻ��룬+xxx,xxx
    }

    public static void RemoveListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);//�ڻ�վ��ɾ��
        EventTable[eventType] = (CallBack<T, X, Y, Z>)EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);//��ֹͨѶ
    }

    public static void Broadcast<T, X, Y, Z>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        if (EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);//����
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    #endregion
}
