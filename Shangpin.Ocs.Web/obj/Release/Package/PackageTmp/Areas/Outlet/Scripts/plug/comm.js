/**  
* ��Date����չ���� Date ת��Ϊָ����ʽ��String  
* ��(M)����(d)��12Сʱ(h)��24Сʱ(H)����(m)����(s)����(E)������(q) ������ 1-2 ��ռλ��  
* ��(y)������ 1-4 ��ռλ��������(S)ֻ���� 1 ��ռλ��(�� 1-3 λ������)  
* eg:  
* (new Date()).pattern("yyyy-MM-dd hh:mm:ss.S") ==> 2007-07-02 08:09:04.423  
* (new Date()).pattern("yyyy-MM-dd E HH:mm:ss") ==> 2007-03-10 �� 20:09:04  
* (new Date()).pattern("yyyy-MM-dd EE hh:mm:ss") ==> 2007-03-10 �ܶ� 08:09:04  
* (new Date()).pattern("yyyy-MM-dd EEE hh:mm:ss") ==> 2007-03-10 ���ڶ� 08:09:04  
* (new Date()).pattern("yyyy-M-d h:m:s.S") ==> 2007-7-2 8:9:4.18  
*/
Date.prototype.dateFormat = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,
        //�·�   
        "d+": this.getDate(),
        //��   
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12,
        //Сʱ   
        "H+": this.getHours(),
        //Сʱ   
        "m+": this.getMinutes(),
        //��   
        "s+": this.getSeconds(),
        //��   
        "q+": Math.floor((this.getMonth() + 3) / 3),
        //����   
        "S": this.getMilliseconds() //����   
    };
    var week = {
        "0": "\u65e5",
        "1": "\u4e00",
        "2": "\u4e8c",
        "3": "\u4e09",
        "4": "\u56db",
        "5": "\u4e94",
        "6": "\u516d"
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    if (/(E+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "\u661f\u671f" : "\u5468") : "") + week[this.getDay() + ""]);
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}
//test   
//alert((new Date()).dateFormat("yyyy-MM-dd EEE hh:mm:ss"));  
function gettimestamp() {
    var unix_time = Math.round(new Date().getTime() / 1000);
    return "timestamp=" + unix_time;
}
function gettimestampurl(url) {
    var s = url.split('?');

    if (s.length > 1) {

        if (s[1].indexOf('timestamp=') >= 0) {
            var ss = s[1].split('&');
            var newurl = s[0];
            for (i = 0; i < ss.length; i++) {
                if (ss[i].indexOf('timestamp=') >= 0) {
                    ss[i] = gettimestamp();
                }
                if (newurl == s[0]) {
                    newurl = newurl + "?" + ss[i];
                }
                else {
                    newurl = newurl + "&" + ss[i];
                }
            }
            return newurl;
        }
        else {
            return url + "&" + gettimestamp();
        }
    }
    else {
        return url + "?" + gettimestamp();
    }
}