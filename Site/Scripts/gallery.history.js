gallery.history = {
    _subscribers : [],
    
    setup : function(rootPath) {
        this._rootPath = rootPath.replace(/([^\/])$/, '$1/');
        this._usingPushState = !!window.history.pushState;
        $(window).hashchange($.proxy(this._broadcast, this));
        
        if (this._usingPushState)
            $(window).bind('popstate', $.proxy(this._broadcast, this));
    },
    
    add : function(album, item) {
        var current = this.current();
        if (current.album === album && current.item === item)
            return false;
        
        if (this._usingPushState) {
            window.history.pushState(
                null, null,
                this._rootPath + album + (item ? "#" + item : "")
            );            
        }
        else {
            window.location.hash = album + (item ? "/" + item : "");
        }
        return true;
    },
    
    subscribe : function(changed) {
        this._subscribers.push(function() {
            var x = this.current();
            changed(x.album, x.item);
        });
    },
    
    _broadcast : function () {
        for (var i = 0; i < this._subscribers.length; i++) {
            this._subscribers[i].call(this);
        }
    },
    
    current : function() {
        var hash = window.location.hash.replace(/^#/, '');
        if (this._usingPushState) {
            var album = window.location.pathname.replace(this._rootPath, "");
            return { album : album, item : hash };
        }
        
        var parts = hash.split('/');
        return { album: parts[0], item : parts[1] };
    }
};