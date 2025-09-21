# 脚本异常处理

## 语法：

```javascript

try{
  //code
}catch(err){
  println(ex.message);
  println(ex.stackTrace);
}
```

## 异常对象

```javascript
{
  message: //消息
  stackTrace: //栈回溯字符串
}
```

## 抛出异常

使用throw([STRING]msg)函数：

```javascript
throw("this is my exception");
```
---

## 注意

1. **async关键字启动的新线程如果发生未处理的异常可能导致脚本引擎崩溃**
2. **不支持Javascript的throw关键字和finally关键字，注意区分**
