//delegate=ί��
//����������Ϊ���ŵ�����ת��

public delegate void CallBack();
//TΪ�������ͣ�����int��arg�Ǳ�������
public delegate void CallBack<T>(T arg);
public delegate void CallBack<T, X>(T arg1, X arg2);
public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg);