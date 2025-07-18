# 字符串类型内置函数

## STRING.toArray()

参数：无参数
返回值：转换成的数组
示例：
```javascript
println("hello".toArray());
//输出 ["h","e","l","l","o"]
```

##STRING.length()

参数：无参数
返回值：字符串长度，单位字符
示例：

```javascript
println("hello".length());
//输出 5
```

## STRING.split()

参数：
- separator (STRING): 分隔符字符串

返回值：分割后的字符串数组

示例：
```javascript
println("a,b,c".split(","));
//输出 ["a","b","c"]
```

## STRING.subString()

参数：
- startIndex (NUM): 起始索引
- count (NUM): 要截取的长度

返回值：截取后的子字符串

示例：
```javascript
println("hello world".subString(6, 5));
//输出 "world"
```

## STRING.contains()

参数：
- substring (STRING): 要查找的子字符串

返回值：布尔值，表示是否包含该子字符串

示例：
```javascript
println("hello".contains("ell"));
//输出 true
```

## STRING.toUniPtr()

参数：无参数
返回值：指向Unicode字符串的指针(PTR类型)

⚠ 返回的PTR类型必须通过freePtr(ptr)释放，否则会造成内存泄漏

示例：
```javascript
var ptr = "hello".toUniPtr();
// 返回指向Unicode字符串的指针
```

## STRING.toAnsiPtr()

参数：无参数
返回值：指向ANSI字符串的指针(PTR类型)

⚠ 返回的PTR类型必须通过freePtr(ptr)释放，否则会造成内存泄漏

示例：
```javascript
var ptr = "hello".toAnsiPtr();
// 返回指向ANSI字符串的指针
```

### 注意事项
1. 所有字符串操作都是基于Unicode编码
2. 索引从0开始计数
3. 指针操作(toUniPtr/toAnsiPtr)返回的指针需要手动释放内存
4. 字符串长度计算的是字符数而非字节数
