window.LifeHack = (function ($) {
    function debounce(fn, delay) {
        let timeoutId;
        return function () {
            const args = arguments;
            clearTimeout(timeoutId);
            timeoutId = setTimeout(function () {
                fn.apply(null, args);
            }, delay);
        };
    }

    function refreshValidation($form) {
        if ($.validator && $.validator.unobtrusive) {
            $form.removeData("validator");
            $form.removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse($form);
        }
    }

    function initAjaxSearch() {
        $("[data-ajax-search]").each(function () {
            const $input = $(this);
            if ($input.data("ajax-ready")) return;
            $input.data("ajax-ready", true);

            const targetSelector = $input.data("target");
            const url = $input.data("search-url");
            const $target = $(targetSelector);

            const runSearch = debounce(function () {
                $target.addClass("lh-loading");

                $.get(url, { q: $input.val() })
                    .done(function (html) {
                        $target.html(html);
                        $target.removeClass("lh-loading");
                        $target.addClass("lh-list-updated");
                        setTimeout(function () {
                            $target.removeClass("lh-list-updated");
                        }, 240);

                        const count = $target.find("tbody tr, .lh-list-item").length;
                        $("[data-result-count]").text("(" + count + ")");
                    })
                    .fail(function () {
                        $target.removeClass("lh-loading");
                    });
            }, 260);

            $input.on("input", runSearch);
        });
    }

    function initAutocomplete() {
        $("[data-autocomplete]").each(function () {
            const $root = $(this);
            if ($root.data("autocomplete-ready")) return;
            $root.data("autocomplete-ready", true);

            const url = $root.data("search-url");
            const $display = $root.find("[data-autocomplete-display]");
            const $value = $root.find("[data-autocomplete-value]");
            const $results = $root.find("[data-autocomplete-results]");
            const $clear = $root.find("[data-autocomplete-clear]");

            function closeResults() {
                $results.removeClass("is-open").empty();
            }

            function selectItem(item) {
                $display.val(item.text);
                $display.attr("data-selected-text", item.text);
                $value.val(item.id).trigger("change");
                closeResults();
                validateField($value);
            }

            function render(items) {
                $results.empty();

                if (!items.length) {
                    $results.append('<div class="lh-autocomplete__empty">Nema rezultata</div>');
                    $results.addClass("is-open");
                    return;
                }

                items.forEach(function (item) {
                    const $button = $("<button/>", {
                        type: "button",
                        class: "lh-autocomplete__item"
                    });

                    $("<span/>", { text: item.text }).appendTo($button);
                    if (item.subtitle) {
                        $("<small/>", { text: item.subtitle }).appendTo($button);
                    }

                    $button.on("mousedown", function (event) {
                        event.preventDefault();
                        selectItem(item);
                    });

                    $results.append($button);
                });

                $results.addClass("is-open");
            }

            const search = debounce(function () {
                const term = $display.val();
                $.getJSON(url, { term: term })
                    .done(render)
                    .fail(closeResults);
            }, 220);

            $display.on("input", function () {
                $value.val("");
                search();
            });

            $display.on("focus", search);

            $display.on("blur", function () {
                setTimeout(function () {
                    if ($display.val() !== $display.attr("data-selected-text")) {
                        $value.val("");
                    }
                    validateField($value);
                    closeResults();
                }, 150);
            });

            $clear.on("click", function () {
                $display.val("").attr("data-selected-text", "");
                $value.val("").trigger("change");
                validateField($value);
                $display.trigger("focus");
            });
        });
    }

    function pad(value) {
        return String(value).padStart(2, "0");
    }

    function toIsoLocal(date) {
        return date.getFullYear() + "-" +
            pad(date.getMonth() + 1) + "-" +
            pad(date.getDate()) + "T" +
            pad(date.getHours()) + ":" +
            pad(date.getMinutes()) + ":00";
    }

    function currentLanguage() {
        return (navigator.languages && navigator.languages[0]) || navigator.language || "hr";
    }

    function formatDateTime(date) {
        const lang = currentLanguage().toLowerCase();
        const day = pad(date.getDate());
        const month = pad(date.getMonth() + 1);
        const year = date.getFullYear();
        const time = pad(date.getHours()) + ":" + pad(date.getMinutes());

        if (lang.startsWith("en")) {
            return month + "/" + day + "/" + year + " " + time;
        }

        return day + "." + month + "." + year + ". " + time;
    }

    function parseDateTime(value) {
        const text = $.trim(value);
        if (!text) return null;

        const hr = text.match(/^(\d{1,2})\.(\d{1,2})\.(\d{4})\.?\s*(\d{1,2})?:?(\d{2})?$/);
        if (hr) {
            return buildDate(hr[3], hr[2], hr[1], hr[4], hr[5]);
        }

        const en = text.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})\s*(\d{1,2})?:?(\d{2})?$/);
        if (en) {
            return buildDate(en[3], en[1], en[2], en[4], en[5]);
        }

        return null;
    }

    function buildDate(year, month, day, hour, minute) {
        const date = new Date(
            Number(year),
            Number(month) - 1,
            Number(day),
            Number(hour || 0),
            Number(minute || 0),
            0
        );

        if (date.getFullYear() !== Number(year) ||
            date.getMonth() !== Number(month) - 1 ||
            date.getDate() !== Number(day)) {
            return null;
        }

        return date;
    }

    function initDateTimeControls() {
        $("[data-datetime]").each(function () {
            const $root = $(this);
            if ($root.data("datetime-ready")) return;
            $root.data("datetime-ready", true);

            const $display = $root.find("[data-datetime-display]");
            const $value = $root.find("[data-datetime-value]");
            const $now = $root.find("[data-datetime-now]");
            const lang = currentLanguage().toLowerCase();

            $display.attr("placeholder", lang.startsWith("en") ? "mm/dd/yyyy HH:mm" : "dd.mm.yyyy. HH:mm");

            if ($value.val()) {
                const initial = new Date($value.val());
                if (!isNaN(initial.getTime())) {
                    $display.val(formatDateTime(initial));
                }
            }

            $display.on("blur", function () {
                const parsed = parseDateTime($display.val());
                if (parsed) {
                    $value.val(toIsoLocal(parsed));
                    $display.val(formatDateTime(parsed));
                } else {
                    $value.val("");
                }

                validateField($value);
            });

            $now.on("click", function () {
                const now = new Date();
                $display.val(formatDateTime(now));
                $value.val(toIsoLocal(now));
                validateField($value);
            });
        });
    }

    function validateField($field) {
        const $form = $field.closest("form");
        if ($form.length && $.validator) {
            if (!$form.data("validator")) {
                refreshValidation($form);
            }
            $field.valid();
        }
    }

    function initBlurValidation() {
        $("form").each(function () {
            const $form = $(this);
            refreshValidation($form);

            $form.find("input, textarea, select").on("focusout change", function () {
                validateField($(this));
            });
        });
    }

    function initForms() {
        initDateTimeControls();
        initAutocomplete();
        initBlurValidation();
    }

    function initMediaUploads() {
        $("[data-media-upload]").each(function () {
            const $root = $(this);
            if ($root.data("media-ready")) return;
            $root.data("media-ready", true);

            const uploadUrl = $root.data("upload-url");
            const listUrl = $root.data("list-url");
            const redirectUrl = $root.data("redirect-url");
            const targetSelector = $root.data("target");
            const accept = $root.data("accept") || "";
            const $target = $(targetSelector);
            const $status = $root.find("[data-media-status]");
            const token = $root.find('input[name="__RequestVerificationToken"]').val();
            
            console.log("Media Upload Init:", {
                uploadUrl: uploadUrl,
                listUrl: listUrl,
                targetSelector: targetSelector,
                token: token ? "present" : "missing"
            });

            if (!token) {
                $status.text("Greška: CSRF token nedostaje!").addClass("text-danger");
                console.error("CSRF token not found for media upload");
                return;
            }
            
            // Koristi postojeći input ako postoji, inače ga kreiraj
            let $input = $root.find('[data-media-input]');
            if ($input.length === 0) {
                $input = $("<input/>", {
                    type: "file",
                    accept: accept,
                    css: { display: "none" },
                    "data-media-input": ""
                }).appendTo($root);
            }
            
            console.log("Using input element:", $input.length > 0 ? "found" : "created");

            function setStatus(message, isError) {
                $status.text(message || "");
                $status.toggleClass("text-danger", !!isError);
            }

            function loadList() {
                $target.addClass("lh-loading");
                $.get(listUrl)
                    .done(function (html) {
                        $target.html(html);
                        bindDeleteButtons();
                    })
                    .always(function () {
                        $target.removeClass("lh-loading");
                    });
            }

            function uploadFile(file) {
                if (!file) return;

                const data = new FormData();
                data.append("file", file);
                data.append("__RequestVerificationToken", token);

                setStatus("Prijenos datoteke je u tijeku...", false);
                $root.addClass("is-uploading");

                console.log("Uploading file:", file.name, "to", uploadUrl);

                fetch(uploadUrl, {
                    method: "POST",
                    body: data
                })
                    .then(async function (response) {
                        console.log("Upload response status:", response.status);
                        
                        if (!response.ok) {
                            let message = "Upload nije uspio (HTTP " + response.status + ").";
                            try {
                                const json = await response.json();
                                if (json.message) message = json.message;
                            } catch (_) {
                                try {
                                    message = await response.text() || message;
                                } catch (__) {
                                    // ignore
                                }
                            }
                            throw new Error(message);
                        }

                        setStatus("Datoteka je spremljena.", false);
                        if (redirectUrl) {
                            window.location.href = redirectUrl;
                            return;
                        }
                        loadList();
                    })
                    .catch(function (error) {
                        console.error("Upload error:", error);
                        setStatus(error.message, true);
                    })
                    .finally(function () {
                        $root.removeClass("is-uploading");
                        $input.val("");
                    });
            }

            function bindDeleteButtons() {
                $target.find("[data-media-delete]").off("click").on("click", function () {
                    const $button = $(this);
                    const deleteUrl = $button.data("delete-url");
                    const data = new FormData();
                    data.append("__RequestVerificationToken", token);

                    $button.prop("disabled", true);

                    fetch(deleteUrl, {
                        method: "POST",
                        body: data
                    })
                        .then(function (response) {
                            if (!response.ok) throw new Error("Brisanje nije uspjelo.");
                            setStatus("Datoteka je obrisana.", false);
                            loadList();
                        })
                        .catch(function (error) {
                            setStatus(error.message, true);
                            $button.prop("disabled", false);
                        });
                });
            }

            $root.on("click", function (event) {
                if ($(event.target).closest("button, a").length) return;
                console.log("Click detected on upload zone");
                if ($input && $input.length) {
                    $input[0].click();
                }
            });

            $root.on("dragover dragenter", function (event) {
                event.preventDefault();
                $root.addClass("is-dragging");
            });

            $root.on("dragleave drop", function (event) {
                event.preventDefault();
                $root.removeClass("is-dragging");
            });

            $root.on("drop", function (event) {
                const file = event.originalEvent.dataTransfer.files[0];
                uploadFile(file);
            });

            $input.on("change", function () {
                console.log("File selected:", this.files.length > 0 ? this.files[0].name : "none");
                if (this.files.length > 0) {
                    uploadFile(this.files[0]);
                }
            });

            loadList();

            console.log("Media upload initialized successfully for:", uploadUrl);
        });
    }

    function initProfileImage() {
        const $profileLink = $(".lh-nav-profile");
        if ($profileLink.length === 0) return;

        const userId = $profileLink.data("user-id");
        if (!userId) return;

        const getImagesUrl = $profileLink.data("get-images-url");
        if (!getImagesUrl) return;

        $.get(getImagesUrl)
            .done(function (html) {
                // Pronađi img element u HTML-u
                const $images = $(html);
                const $img = $images.find("img").first();
                
                if ($img.length) {
                    const imgSrc = $img.attr("src");
                    const imgAlt = $img.attr("alt");
                    
                    // Zameni ikonu sa slikom
                    $profileLink.html(
                        `<img src="${imgSrc}" alt="${imgAlt}" style="width:40px;height:40px;border-radius:50%;object-fit:cover;" />`
                    );
                }
            });
    }

    return {
        initAjaxSearch: initAjaxSearch,
        initForms: initForms,
        initMediaUploads: initMediaUploads,
        initProfileImage: initProfileImage
    };
})(jQuery);
