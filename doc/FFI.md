# FFI文档

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



