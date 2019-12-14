document.write('<script type="text/javascript" src="/Areas/Outlet/Scripts/plug/jquery.zclip.js"><\/script>');
$(function () {
    $('.copySpan').each(function () {
        var self = $(this);
        var value = self.attr("copyTxt");
        var copy = self.zclip({
            'path': '/Areas/Outlet/Scripts/plug/ZeroClipboard.swf', //这里写自己存在的地址
            'afterCopy': function () {
                alert("已经复制到剪切板,可以使用ctrl+v粘贴\r\n"+value);
            },
            'copy': function () {
                return value; 
            }
        });
    });
}); 