# 对象类型内置函数

## OBJECT.addMember()

参数：
- name (STRING): 成员名称
- value (ANY): 成员值

返回值：添加的成员值

示例：
```javascript
var obj = {};
obj.addMember("name", "John");
println(obj.name); 
println(obj["name"]); //下标方式取值
// 输出 "John"
```

## OBJECT.removeMember()

参数：
- name (STRING): 要移除的成员名称

返回值：null

示例：
```javascript
var obj = {age: 20};
obj.removeMember("age");
println(obj.age);
// 报错
```

## OBJECT.values()

参数：无参数
返回值：包含对象所有值的数组

示例：
```javascript
var obj = {a: 1, b: 2};
var vals = obj.values();
println(vals);
// 输出 [1, 2]
```

## OBJECT.keys()

参数：无参数
返回值：包含对象所有键的字符串数组

示例：
```javascript
var obj = {x: 10, y: 20};
var keys = obj.keys();
println(keys);
// 输出 ["x", "y"]
```

## OBJECT.toPointer()

参数：
- typeName (STRING): 对象类型名称

返回值：指向对象的指针(PTR类型)

示例：
```javascript
var obj = {width: 100, height: 200};
var ptr = obj.toPointer("width:uint,height:uint");
// 返回指向对象的指针
```
关于如何约定原生类型，参见FFI文档

### 注意事项

1. 对象成员操作是动态的，可以随时添加或删除
2. values()和keys()返回的数组顺序不保证与定义顺序一致
3. toPointer()需要传入正确的类型名称以便进行正确的类型转换
4. 指针操作(toPointer)返回的指针需要谨慎管理生命周期
