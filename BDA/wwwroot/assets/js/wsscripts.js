function SetDevExtremeDisabled(divid) {
    var allWidgets = $(divid).find(".dx-widget");    
    $.each(allWidgets, function (i, v) {
        var data = $(v).data();
        if (typeof (data.dxComponents) != 'undefined') {
            var dxComponentName = data.dxComponents[0];
            if (dxComponentName.indexOf("dxPrivateComponent") < 0) {
                var value = $(v)[dxComponentName]("instance").option("readOnly");
                if (!value) {

                    if (dxComponentName == "dxHtmlEditor" || dxComponentName == "dxTextBox" || dxComponentName == "dxTextArea" ||
                        dxComponentName == "dxTagBox" || dxComponentName == "" ||
                        dxComponentName == "dxNumberBox" || dxComponentName == "dxCheckBox") {
                        $(v)[dxComponentName]("instance").option("readOnly", true);
                    }

                    if (dxComponentName == "dxSelectBox") {
                        if ($(v)[dxComponentName]("instance").option("name") != "") {
                            $(v)[dxComponentName]("instance").option("readOnly", true);
                        }
                    }

                    if (dxComponentName == "dxDateBox") {
                        if ($(v)[dxComponentName]("instance").option("name") != "") {
                            $(v)[dxComponentName]("instance").option("disabled", true);
                        }
                    }

                    if (dxComponentName == "dxButton") {
                        if ($(v)[dxComponentName]("instance").option("name") != "") {
                            $(v)[dxComponentName]("instance").option("disabled", true);
                        }
                    }
                }
            }
        }
    })
}

//set array of string elemen devextreme menjadi required / tidak
function SetValidationRequired(ids, isRequired) {
    var i;
    for (i = 0; i < ids.length; i++) {
        var elemID = "#" + ids[i];
        if (isRequired) {
            var prevLabel = $(elemID).prevAll("label").text();
            $(elemID).dxValidator({
                validationRules: [{ type: "required", message: (prevLabel + " harus diisi") }]
            });
        } else {
            $(elemID).dxValidator({
                validationRules: []
            });
        }
    }
}


// Jam Server
var serverTime = null;
function SetCurrentTime(time) {
    serverTime = time;
}
function ShowTime() {
    var serverDateTime = new Date(serverTime);
    var monthNum = serverDateTime.getMonth() + 1;
    var t = serverDateTime.getDate();
    var y = serverDateTime.getFullYear();
    var h = serverDateTime.getHours();
    var m = serverDateTime.getMinutes();
    var s = serverDateTime.getSeconds();
    var bulan = namaBulan(monthNum);
    h = checkTime(h);
    m = checkTime(m);
    s = checkTime(s);
    document.getElementById("lblTime").innerHTML = t + " " + bulan + " " + y + " " + h + ":" + m + ":" + s + " WIB";
    serverTime = new Date(serverDateTime.setSeconds(serverDateTime.getSeconds() + 1));
    setTimeout(function () { ShowTime() }, 1000);
}
function namaBulan(month) {
    if (month == 1)
        monthname = "Januari";
    else if (month == 2)
        monthname = "Febuari";
    else if (month == 3)
        monthname = "Maret";
    else if (month == 4)
        monthname = "April";
    else if (month == 5)
        monthname = "Mei";
    else if (month == 6)
        monthname = "Juni";
    else if (month == 7)
        monthname = "Juli";
    else if (month == 8)
        monthname = "Agustus";
    else if (month == 9)
        monthname = "September";
    else if (month == 10)
        monthname = "Oktober";
    else if (month == 11)
        monthname = "November";
    else
        monthname = "Desember";
    return monthname;
}
function checkTime(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}
function clearTimeOut() {
    window.clearTimeout(ShowTime());
}
