## 注意事项：

1. 由于renderGraph在进行逆序列化的时候，会构建所有的pass，所以请确保pass内所有变量的值初始化符合unity的条件（不涉及UnityEngine.Object基本都可以），否则会报这样的错误![image-20220328085646966](C:\Users\hgx\Desktop\graphics\image-20220328085646966.png)![image-20220328085607672](C:\Users\hgx\Desktop\graphics\image-20220328085607672.png)对于这些变量，可以放在AllocateWriteResource函数中进行new![image-20220328090057925](C:\Users\hgx\Desktop\graphics\image-20220328090057925.png)（也可以利用个单例类管理这些，应该吧。。)

2. 所有资源（texture，computeBuffer，rendererList）务必带有**PortPinAttribute**，对于所有带有write的资源，务必在AllocateWriteResource函数内使用相关函数进行资源创建。（PS:对于read及readWrite不用管事。）

3. 如果在连接图的时候没有把所有read全部都连接的话，目前默认是Cull掉该pass，哪怕是allowpasscull为false。

   