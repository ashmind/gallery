global = {
    lightboxNotYetShown : true,
    noHistoryRecordWhileClosingLightbox : false
};

$(function() {
    $.history.init(function(hash) {
        if ($.history.noaction)
            return;

        var parts = hash.replace(/^\\/, '').split('\\');
        var albumLink = $("#left a.album-name[data-id='" + parts[0] + "']");

        if (albumLink.length == 0)
            return;

        var selectItem = function() {
            if (parts.length == 1)
                return;
            
            var a = $(".wall a[data-name='" + parts[1] + "']");
            $("#main").scrollTop(a.position().top);
            
            $.history.noaction = true;
            if (global.lightboxNotYetShown) {
                a.click();
            }
            else {
                a.parent().toggleClass('selected');
            }
            $.history.noaction = false;
        };

        global.noHistoryRecordWhileClosingLightbox = true;
        $.fancybox.close();

        if (!albumLink.hasClass('selected')) {
            loadAlbum(albumLink, selectItem);
        }
        else {            
            selectItem();
        }
    });
    setupAlbum();

    $("#left a.album-name").live('click', function(event) {
        event.preventDefault();
        $.history.load("\\" + $(this).attr('data-id'));
    });

    $("#left section.folders a.more").live('click', function(event) {
        event.preventDefault();

        var a = $(this);
        $.get(a.attr('data-ajax'), {}, function(html) {
            a.replaceWith(html);
        });
    });

    $("#left section.people a.more").click(function() {
        $(this).remove();
        $("#left section.people div.more").show();        
    });
});

function loadAlbum(a, onsuccess) {
    $.get(a.attr('href'), {}, function(html) {
        $('#main').html(html);

        $('#left').find('.selected').removeClass('selected');
        a.addClass('selected');

        setupAlbum();

        document.title = document.title.replace(/:.+$/, "") + ": " + a.text();

        if (onsuccess)
            onsuccess();
    });        
}

function setupAlbum() {
    $(".wall a").fancybox({
        padding	        : 0,
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        titlePosition   : 'over',
        titleFormat     : function(title, currentArray, currentIndex) {
            var a = currentArray.eq(currentIndex);

            var locate = "";
            var primaryAlbumID = a.attr('data-primaryAlbumID');
            if (primaryAlbumID) {
                locate = "<a class='locate' " +
                    "data-albumID='" + primaryAlbumID + "' " +
                    "data-itemname='" + a.attr('data-name') + "' " +
                    "href='javascript:void(\"locate\")'" +
                ">Locate</a>";
            }

            var _delete = "";
            var deleteAction = a.attr('data-action-delete')
            if (deleteAction) {
                _delete = "<a class='delete' href='" + a.attr('data-action-delete') + "'>Propose to delete</a>";
            }

            return "<span id='fancybox-title-over' class='image-actions'>" +
                locate +
                _delete +
                "<a href='" + a.attr('data-action-download') + "'>Download</a>" +
                "<a href='" + a.attr('data-action-comment') + "'>Comment</a>"
            "</span>";
        },
        onComplete      : function(currentArray, currentIndex) {
            global.lightboxNotYetShown = false;

            var a = currentArray.eq(currentIndex);
            a.parent().toggleClass('selected');
            if (!$.history.noaction) {
                $.history.noaction = true;
                $.history.load("\\" + a.parents('.album-view').attr('data-id') + '\\' + a.attr('data-name'));
                $.history.noaction = false;
            }

            $("#fancybox-wrap .locate").click(function(event) {
                event.preventDefault();                
                $.history.load("\\" + $(this).attr('data-albumid') + '\\' + $(this).attr('data-itemname'));
            });

            $("#fancybox-wrap .delete").click(function(event) {
                event.preventDefault();                
                $.get($(this).attr('href'), {}, function(html) {
                    $.fancybox.close();
                });
            });
        },
        onClosed        : function(currentArray, currentIndex) {
            var a = currentArray.eq(currentIndex);

            if (!global.noHistoryRecordWhileClosingLightbox) {
                $.history.noaction = true;
                $.history.load("\\" + a.parents('.album-view').attr('data-id'));
                $.history.noaction = false;
            }
            global.noHistoryRecordWhileClosingLightbox = true;
        }
    });

    $("#main").scrollTop(0);
    var imgs = $(".wall img");
    imgs.lazyload({
        placeholder : imgs.eq(0).attr('src'),
        container   : $("#main"),
        effect      : "fadeIn"
    });

    setupSecurityPanel();

    /*$('.wall').dragToSelect({
        selectables  : '.item',
        selectOnMove : true/ *,
        onHide       : function () {
            var selected = $('.wall .item.selected');
            if (selected.length > 0) {
                $("#main").css('right', '12.6em');
                $("#right-menu").show();
            }
            else {
                $("#right-menu").hide();
                $("#main").css('right', '0');
            }
        }* /
    });*/
}

function setupSecurityPanel() {
    $("a.album-visible-to").fancybox({
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        onComplete      : function() {
            $('form.grant img')
                .click(function() {
                    var group = $(this).parent();

                    group.toggleClass('granted');
                    var hidden = group.find('input');
                    hidden.val(hidden.val() == "" ? group.attr('data-key') : "");
                });

            $('form.grant').ajaxForm({
                success : function() {
                    $.fancybox.close();
                    loadAlbum($("#left a.album-name.selected")); // reloads album
                }
            });
        }
    });
}