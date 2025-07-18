# 基本语法文档（中文）


## 一、基础语法结构

### 1. 变量定义
```javascript
// 类型自动推导
var a = 1;          // 数字 NUM类型
var b = 3.14;        // 数字 NUM类型
var c = "hello";     // 字符串 STRING类型
var d = true;        // 布尔值 BOOL类型
var f = function(a,b){return a + b;}; //匿名函数 FUNCTION类型

```

### 2. 控制语句
#### 条件分支
```javascript
if (score > 90) {
    println("优秀");
} else if (score > 60) {
    println("及格"); 
} else {
    println("不及格");
}
```

#### 循环结构

```javascript
// 范围循环
for (i : range(0, 5,1)) {
    print(i + " ");  // 输出: 0 1 2 3 4
}
```
**range函数定义：range(start,end,step)，返回的数组范围包含start不包含end**

```javascript
//条件循环
var a = 0;
while(a < 10){
   a++;
}
```

### 3.异常处理

```javascript
try{
   //可能会出错的代码
}catch(ex){
   println(ex); //捕获到的异常ex为STRING类型
}
```

### 4.函数定义

```javascript
function funcName(arg1,arg2){
   //函数代码
   return result;
}
```
如果没有在函数中返回任何值，那么默认返回null

### 5.集合定义

```javascript
var array = [1,2.5,"hello",true,[5,6,7]];
println(arr[2]);  //输出hello
println(arr[4][0]); //输出5
```

### 6.对象定义

```javascript
//定义对象
var obj = {
  key1: "strValue",
  key2: 123,
  key3: { test : true }
}
```
