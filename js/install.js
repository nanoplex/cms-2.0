(function() {
    "use strict";

    var installer = document.getElementById("installer");

    installer.Loglevel = "2";

    Object.observe(installer, function(change) {
        for (var i = 0; i < change.length; i++) {
            if (change[i].name == "Loglevel") {
                var level = document.querySelector("input[type=range]").value;

                if (level === "0")
                    document.getElementById("loglevelstauts").innerHTML = "Error only";
                else if (level === "1")
                    document.getElementById("loglevelstauts").innerHTML = "Warnings";
                else if (level === "2")
                    document.getElementById("loglevelstauts").innerHTML = "All events";
            }
        }
    });

    installer.ShowUser = function() {
        this.$.welcome.setAttribute("hidden", "");
        this.$.newUser.removeAttribute("hidden");
    };
    installer.ShowSite = function() {

        this.$.userStatus.innerHTML = "<paper-spinner active></paper-spinner>";

        if (document.querySelector("input[name=username]").validity.valid &&
            document.querySelector("input[name=useremail]").validity.valid &&
            document.querySelector("input[name=userpassword]").validity.valid &&
            this.UserPass == this.passrepeat) {
            this.$.ajaxUser.go();
        } else {
            this.$.userStatus.innerHTML = "all fields are required and email must be valid";
        }

    };
    installer.HandleUser = function(res) {

        if (res.detail.response.match(/true/) != null) {
            this.$.newUser.setAttribute("hidden", "");
            this.$.newSite.removeAttribute("hidden");
        } else
            this.$.userStatus.innerHTML = res.detail.response;
    };
    installer.HandleSite = function(res) {
        if (res.detail.response.match(/true/) != null)
            window.location.href = "/index.html";
        else
            this.$.siteStatus.innerHTML = res.detail.response;
    };
    installer.Finish = function() {
        this.$.siteStatus.innerHTML = "<paper-spinner active></paper-spinner>";

        if (document.querySelector("input[name=sitename]").validity.valid)
            this.$.ajaxSite.go();
        else
            this.$.siteStatus.innerHTML = "you must enter a name for your project";
    };
})();