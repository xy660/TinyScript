# 数组类型内置函数

## ARRAY.add()

参数：
- element (ANY): 要添加的元素

返回值：修改后的数组(ARRAY)

示例：
```javascript
var arr = [1, 2];
arr.add(3);
println(arr);
// 输出 [1, 2, 3]
```

## ARRAY.length()

参数：无参数  
返回值：数组长度(NUM)

示例：
```javascript
var arr = ["a", "b", "c"];
println(arr.length());
// 输出 3
```

## ARRAY.insert()

参数：
- index (NUM): 要插入的位置索引
- element (ANY): 要插入的元素

返回值：修改后的数组(ARRAY)

示例：
```javascript
var arr = [10, 20, 30];
arr.insert(1, 15);
println(arr);
// 输出 [10, 15, 20, 30]
```

## ARRAY.remove()

参数：
- index (NUM): 要移除的元素索引

返回值：修改后的数组(ARRAY)

示例：
```javascript
var arr = ["x", "y", "z"];
arr.remove(1);
println(arr);
// 输出 ["x", "z"]
```

## ARRAY.sort()

参数：无参数  
返回值：排序后的数组(ARRAY)

示例：
```javascript
var arr = [3, 1, 4, 2];
arr.sort();
println(arr);
// 输出 [1, 2, 3, 4]
```
排序规则：混合排序，NUM类型权重为自身，STRING类型权重为字符串长度，BOOL类型权重为true=1,flase=0

### 注意事项

1. 所有索引操作从0开始计数
2. 索引越界会抛出异常
3. sort()函数会对数组元素进行原地排序
4. 数组可以包含任意类型的元素混合
5. 修改操作(add/insert/remove/sort)都会返回数组本身，支持链式调用
