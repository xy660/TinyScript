# 异步/多线程

## async关键字

可通过async关键字启动新任务：
```javascript
async <表达式>;
```
或者
```javascript
var promise = async <表达式>;
```

示例代码：
```javascript
var work = function(){sleep(1000);println("delay 1000 ms print")};
var promise = async work();
promise.wait();
```
## lock关键字

可通过lock关键字对代码块加锁，防止多线程同时访问同一资源造成问题

用法：
```javascript
var lockobj = 0;
lock(lockobj)
{
  //code to lock
}
```
lockobj可以是任意对象，任意类型

示例代码：
```javascript
var myLock = "my_lock";
var a = 0;
var func = function(lockobj){while(true){lock(lockobj){a++}}};
var p1 = async func(myLock);
var p2 = async func(myLock);
```

示例代码中任务1和任务2通过lock关键字加锁防止同时访问a出现问题



