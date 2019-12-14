/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 13-7-19
 * Time: 下午1:30
 * To change this template use File | Settings | File Templates.
 */
;
(function (window, $, MXTempl, undefined) {
    MXTempl("sp_brfs")
        .render(function () {
            var data = this.data,
                html = data.replace(/\$\$content\{(\w+)\}/gi, "<i class=\"sp_brfs_listnum\">$1</i>"),
                _this = this;
            this.setHTML(html);
        })
        .extend("update", function(id, content, html){
            var data = this.data,
                reg = new RegExp("\\$\\$content\\{" + id + "\\}", "gi"),
                update = data.replace(reg, content);
            //console.log(html);
            this.data = update;
            this.html = html;
            return this;
        });
})(window, jQuery, MXTempl);