# TinyScript

 **一个轻量级脚本语言** | **A lightweigtht scripting language**  

可作为脚本引擎嵌入您的C#程序或作为脚本引擎编写例子

---

## 特性 | Features  

✅ Lightweight syntax / 轻量级语法 

✅ ​​Object-Oriented / 面向对象

✅ Easy embedding in applications / 易于嵌入应用程序  

✅ Learn basics in 10 minutes / 10分钟掌握基础

✅ Cross-platform / 跨平台

---

## 快速示例 | Quick Examples  

### 1. FFI互操作 | FFI interop
```javascript
var beep = ffiload("kernel32.dll","Beep","int,int","int");
beep(2000,1000);

```

### 2. for-each循环 | for-each loop
```javascript
for(i : range(0,10,1)) //range(start,end,step) return include [start,end)
{
    println(i);
}
println("");
for(i : range(0,10,2))
{
    println(i);
}
println("");
var arr = ["boo","foo","hello","world"];
for(i : arr)
{
    println(i);
}
readln();
```
*Output:*
```bash
0
1
2
3
4
5
6
7
8
9

0
2
4
6
8

boo
foo
hello
world
```

### 3.函数定义 | function definitions
```javascript
function a(y)
{
    println("a() called => " + y);
}

a("function a");
readln(); //wait
```

### 4.文件操作 | File operations
```javascript
var a = readText("./your/path/to.txt"); //读取文本文件 Read a txt file as string
var b = listFile("./your/path"); //遍历文件夹文件 Traversing directory files
var c = listDir("./your/path"); //遍历子文件夹 Traversing Subdirectories
```

### 5.异步任务 | Async task
```javascript
var work = function(){
    sleep(1000);
    println("print from async task");
    return 123;
}
var promise = async work();
println("task result: " + promise.getResult());
```

--- 
## Get start

 1. 运行脚本文件 | Run script from file
```bash
ScriptRuntime your_script.tiny
```

 2. CLI模式 | CLI mode

```bash
./ScriptRuntime
TinyScript REPL CLI Version V1.0.0

TinyScript>

```

---

# 如何在脚本引擎中增加自己的内置函数：

## 在src/ScriptRuntime/Runtime/SystemFunctions.cs：

**Example：**

创建自定义系统函数
```csharp
public static VariableValue PrintLine(List<VariableValue> args)
{
    Console.WriteLine(args[0].Value.ToString());
    return EmptyVariable;
}
```

注册到函数表中
```csharp
FunctionTable.Add("println", new ScriptFunction("println",
    new List<ValueType>() { ValueType.ANY }, ValueType.NULL, PrintLine));
```

---

<!--[知乎文章(1)](https://zhuanlan.zhihu.com/p/1929188026011058240)
[知乎文章(2)](https://zhuanlan.zhihu.com/p/1929561902754828527)-->



