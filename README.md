# Note
Ajax
页面加载查询留言表数据，显示到页面上 要求使用ajax无刷新查询， 服务器返回JSON到浏览器端 ，在客户端遍历JSON里面的数据动态创建元素显示在页面上
如图：


在名字和内容文本框填写数据后，点击发送消息按钮，数据被添加到数据库，同时页面数据进行更新 ，但是不能刷新页面。


内容和名字文本框在获得焦点的时候要求实现选中效果：
提示使用js实现，onfocus="this.select();"

所有留言的内容后面显示留言时间，以及在留言内容后面追加删除超链接，点击删除超链接实现删除当前留言内容删除效果，同时数据库数据也被删除，页面无刷新
