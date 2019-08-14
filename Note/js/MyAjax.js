function MyAjax(method, url, C) {
	var xmlhttp = null;
	//内核判断
	if (window.ActiveXObject) {
		//IE
		xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
	} else {
		//!IE
		xmlhttp = new XMLHttpRequest();
	}
	if (!xmlhttp) {
		alert("您的浏览器不支持Ajax技术，请更换其它浏览器");
		return false;
	}

	xmlhttp.open(method, url, false);
	xmlhttp.onreadystatechange = function () {
		if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
			var res = xmlhttp.responseText;
			C(res);
		}
	}
	xmlhttp.send(null);
}