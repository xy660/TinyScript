# FFI文档

## 加载外部动态链接库

示例：

```javascript
var func = ffiload("lib_name","func_name","arg_type1,arg_type2...","return_type");
var result = func(...);
```
对于如何描述参数和返回值的原生类型封送，见下文

## 将脚本函数暴露为函数指针（动态生成桩）

此功能目前仅支持X86 X64架构CPU的Linux，Windows操作系统，当前为实验性功能

示例：

```javascript
var pAdd = createCallback({
    func:function(a,b){
        return a+b;
    },
    argTypes:"uint,uint",
    retTypes:"uint",
    abi:"cdecl" //32位系统可选，64位系统不需要指定，自动按照系统默认调用约定
})

freePtr(pAdd); //使用完毕后使用freePtr函数结束其生命周期
```
在外部Native层，例如Clang这样调用：
```c
typedef int (*AddFuncPtr)(int, int);
.......
AddFuncPtr add_func = (AddFuncPtr)pf;
int result = add_func(123,456);
```
参数类型描述和返回值类型描述见下函数签名类型表。

X64下无需指定ABI，FFI引擎会自动选择ABI，X86下若未指定abi则Windows下会自动使用Stdcall，Linux下自动使用Cdecl

ABI一共支持以下字符串指定：

- stdcall
- cdecl
- win64
- systemv

如果显式指定abi，请确保你的调用方使用和你匹配的调用约定，否则将会导致进程崩溃或未定义行为

## 结构体约定表达式

格式：
```javascript
"name1:type1,name2:type2[array_length],name3:type3..."
```
支持的类型：

| 原生类型 | 脚本类型 |
|----------|----------|
| string   | STRING   |
| ptr      | PTR      |
| int      | NUM      |
| long     | NUM      |
| uint     | NUM      |
| ulong    | NUM      |
| short    | NUM      |
| ushort   | NUM      |
| byte     | NUM      |
| float    | NUM      |
| double   | NUM      |
| bool     | BOOL     |

## 函数签名约定表达式

格式：

```javascript
var func = ffiload("lib_name","func_name","arg_type1,arg_type2...","return_type");
```

支持的类型：

| 原生类型 | 脚本类型 |
|----------|----------|
| string   | STRING   |
| ptr      | PTR      |
| int      | NUM      |
| long     | NUM      |
| uint     | NUM      |
| ulong    | NUM      |
| short    | NUM      |
| ushort   | NUM      |
| byte     | NUM      |
| float    | NUM      |
| double   | NUM      |
| bool     | BOOL     |
| void     | NULL     |
| data     | ARRAY(默认全是byte类型)|


## 内存管理

- OBJECT.toPointer()
- STRING.toUniPtr()
- STRING.toAnsiPtr()
- createCallback()

都会申请堆内存，使用完毕后需要及时释放

通过调用freePtr进行释放：

```javascript
var ptr = ...;
freePtr(ptr);
```

## 注意事项

1. 错误的函数/结构体约定会导致程序崩溃
2. 重复释放指针可能会导致程序崩溃
3. 未及时释放指针可能会导致内存泄漏



