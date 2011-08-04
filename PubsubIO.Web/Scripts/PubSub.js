(function() {
    var BOSH_SERVICE = 'http://matcctst09.net.dr.dk:5280/http-bind';
    var PUBSUB_SERVICE = 'pubsub.matcctst09.net.dr.dk';

    //var BOSH_SERVICE = 'http://drejabberd.tklapp.com:5280/http-bind';
    //var PUBSUB_SERVICE = 'pubsub.drejabberd.tklapp.com';

    var NS_DATA_FORMS= "jabber:x:data";
    var NS_PUBSUB= "http://jabber.org/protocol/pubsub";
    var NS_PUBSUB_OWNER= "http://jabber.org/protocol/pubsub#owner";
    var NS_PUBSUB_ERRORS= "http://jabber.org/protocol/pubsub#errors";
    var NS_PUBSUB_NODE_CONFIG= "http://jabber.org/protocol/pubsub#node_config";

    //var connection = null;

    // The following adds logging output from Strophe. Beware it is verbose.
    //Strophe.log = function (level, msg) { log('LOG: ' + msg); };

    var log = function(msg) {
        //console.log(msg);
    };

    Pubsub = new Class({
        Implements: Events
        ,initialize: function(jid, password){
            this.jid = jid;
            this.password = password;
            this.service = PUBSUB_SERVICE;
            this.connection = new Strophe.Connection(BOSH_SERVICE);
        }
        ,onConnect: function() {
            // send negative presence send we’re not a chat client
            log("connected");
            this.fireEvent('connected');
            this.connection.send($pres().c("priority").t("-1"));
        }
        ,connect: function(){
            this.connection.connect(this.jid, this.password, (function (status) {
                if (status === Strophe.Status.CONNECTED) {
                    this.onConnect();
                } else if (status === Strophe.Status.DISCONNECTED) {
                    this.onDisconnect();
                }
            }).bind(this));;
        }
        ,disconnect: function(){
            this.connection.disconnect();
        }
        ,subscribe: function(nodeName, eventHandler) {
            this.addEvent(nodeName, eventHandler);
            //console.log('in subscribe', arguments);

            
            // a node was specified, so we attempt to subscribe to it

            // first, set up a callback for the events
            
            //console.log('connection:', this.connection, this.connection.addHandler, this.service);
            this.connection.addHandler(
                    this.onMessage.bind(this),
                    null,
                    "message",
                    null,
                    null,
                    this.service, null);

            // now subscribe
            var subiq = $iq({to: this.service,
                             type: "set"})
                .c('pubsub', {xmlns: NS_PUBSUB})
                .c('subscribe', {node: nodeName,
                                 jid: this.connection.jid});

            this.connection.sendIQ(subiq,
                                   this.onSubscribe.bind(this),
                                   this.onSubscribeError.bind(this));
        }
        ,onSubscribe: function(){
        }
        ,onSubscribeError: function(){
        }
        ,onMessage: function(msg) {
            var nodeName = $(msg).getElement('items').get('node');
            var items = $(msg).getElement('items')
                              .getElements('item')
                              .each((function(item) {
                                          this.fireEvent(nodeName, item);
                                     }).bind(this));

            // return true to ensure continued handling of messages
            return true;
        }
    });
})();

