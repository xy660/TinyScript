# 指针类型内置函数

## PTR.asUniString()

参数：无参数  
返回值：从Unicode指针转换的字符串(STRING)  

示例：
```javascript
var ptr = "hello".toUniPtr();
var str = ptr.asUniString();
println(str);
// 输出 "hello"
```

## PTR.asAnsiString()

参数：无参数  
返回值：从ANSI指针转换的字符串(STRING)  

示例：
```javascript
var ptr = "world".toAnsiPtr();
var str = ptr.asAnsiString();
println(str);
// 输出 "world"
```

## PTR.asObject()

参数：
- typeName (STRING): 原结构体定义  

返回值：从指针转换的对象(OBJECT)  

示例：
```javascript
var obj = {width: 100, height: 200};
var ptr = obj.toPointer("width:uint,height:uint");
var newObj = ptr.asObject("width:uint,height:uint");
println(newObj.width);
// 输出 100
```

关于如何约定结构体定义，参见FFI文档

### 注意事项

1. 指针转换操作需要确保指针类型与转换目标类型匹配
2. 字符串转换会自动处理空指针情况，返回"null"字符串
3. 对象转换需要提供正确的类型名称以便进行正确的类型转换
4. 转换后的对象是原对象的副本，修改不会影响原指针数据
5. 指针生命周期需要自行管理，转换操作不会释放指针内存
