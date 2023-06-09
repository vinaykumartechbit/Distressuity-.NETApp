window.setTimeout(function () {
    jQuery("#example1").radialProgress("init", {
        'size': 100,
        'fill': 5
    }).radialProgress("to", { 'perc': parseInt(jQuery("#example1").attr('value')), 'time': 2000 })
}, 2000);




//
//var startClock = function() {
//  var dh, dm, ds;
//  setInterval(function() {
//    var date = new Date(),
//        h = date.getHours() % 12,
//        m = date.getMinutes(),
//        s = date.getSeconds();
//    if (dh !== h) { clock.radialMultiProgress("to", {
//      "index": 0, 'perc': h, 'time': (h ? 100 : 10)
//    }); dh = h; }
//    if (dm !== m) { clock.radialMultiProgress("to", {
//      "index": 1, 'perc': m, 'time': (m ? 100 : 10)
//    }); dm = m; }
//    if (ds !== s) { clock.radialMultiProgress("to", {
//      "index": 2, 'perc': s, 'time': (s ? 100 : 10)
//    }); ds = s; }
//  }, 1000);
//};
//
//startClock();


