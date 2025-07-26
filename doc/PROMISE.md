# PROMISE类型内置函数

## PROMISE.getResult()

参数：无

返回值：(ANY)任务运行结果

示例：
```javascript
var p = async sin(PI);
println(p.getResult());
```
 * **如果任务还在运行中，则会阻塞直到任务返回**

## PROMISE.isRunning()

参数：无

返回值：(BOOL)表示任务是否正在运行

## PROMISE.wait()

参数：无

返回值：(ANY)任务运行结果

## PROMISE.waitTimeout()

参数：
- (NUM)等待的毫秒数

返回值：(BOOL)如果为true则任务在等待的时间之前结束，否则为false

