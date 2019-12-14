KindEditor.ready(function (K) {
    window.editor1 = K.create('#HeadWebHtml', {
        width: 500,
        height: 300,
        uploadJson: '/Outlet/Subject/EditorSaveUploadPic',
        //fileManagerJson: '/Outlet/Subject/EditorBrowse',
        allowFileManager: false,
        allowImageManager: true,
        afterCreate: function () {
            var self = this;
            K.ctrl(document, 13, function () {
                self.sync();
                //K('form[name=example]')[0].submit();
            });
            K.ctrl(self.edit.doc, 13, function () {
                self.sync();
                // K('form[name=example]')[0].submit();
            });
        },
        extraFileUploadParams: {
            "AUTHID": $("#authid").val()
        }
    });
    //prettyPrint();
    // alert("这是里面的" + editor1.html());
});


KindEditor.ready(function (K) {
    window.editor2 = K.create('#HeadMobileHtml', {
        width: 500,
        height: 300,
        uploadJson: '/Outlet/Subject/EditorSaveUploadPic',
        //fileManagerJson: '/Outlet/Subject/EditorBrowse',
        allowFileManager: false,
        allowImageManager: true,
        afterCreate: function () {
            var self = this;
            K.ctrl(document, 13, function () {
                self.sync();
                //K('form[name=example]')[0].submit();
            });
            K.ctrl(self.edit.doc, 13, function () {
                self.sync();
                // K('form[name=example]')[0].submit();
            });
        },
        extraFileUploadParams: {
            "AUTHID": $("#authid").val()
        }
    });
    // prettyPrint();
});