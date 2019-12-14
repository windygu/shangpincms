/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 13-7-19
 * Time: 下午3:51
 * To change this template use File | Settings | File Templates.
 */
;(function(window, $, SPModeWindowCollection){
    var brfsWindow = function(setting){
        var defaultSetting = {
                title: "",
                content: "",
                binder: null,
                backover: true
            },
            extendSetting = $.extend({}, defaultSetting, setting);

        return SPModeWindowCollection.add("sp_modewindow_brfs", {
            style: {
                wrapper : "sp_modewindow_brfs",
                title : "sp_modewindow_brfs_title",
                content :"sp_modewindow_brfs_content",
                bottom : "sp_modewindow_brfs_bottom"
            },
            html: {
                title : "区域编辑",
                content : "<p>上传图片：<input type='file' id='sp_brfs_edit_imgfile' name='sp_brfs_edit_imgfile' /></p><p>图片链接：<input type='text' id='sp_brfs_edit_imgurl' name='sp_brfs_edit_imgurl' /></p><div class='sp_brfs_edit_wrapper'><a href='#' id='sp_brfs_edit_submit'>确定</a>&nbsp;&nbsp;<a href='#' id='sp_brfs_edit_cancel'>取消</a></div>"
            },
            binder:function(){
                var _this = this;
                extendSetting.binder && extendSetting.binder.call(this);
                $("#sp_brfs_edit_cancel").on("click", function(){
                    _this.close();
                    return false;
                });
            },
            backover: extendSetting.backover
        });
    };

    window.$brfsWindow = brfsWindow;
})(window, jQuery, SPModeWindowCollection)