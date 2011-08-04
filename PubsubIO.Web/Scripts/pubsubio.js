var pubsub = pubsubio.connect('matcctst09:9999/dr');
pubsub.subscribe({
    hello: { $any: ['world', 'mundo', 'verden'] }
}, function (doc) {
    alert(doc.hello);
});
//pubsub.publish({ hello: 'world' });