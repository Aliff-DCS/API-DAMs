document.addEventListener("DOMContentLoaded", () => {
    const nav = document.querySelector("nav");
    const offset = 100; // Adjust this value to set when the nav becomes fixed

    window.addEventListener("scroll", () => {
        if (window.scrollY > offset) {
            nav.classList.add("fixed");
        } else {
            nav.classList.remove("fixed");
        }
    });
});

window.onbeforeunload = function () {
    localStorage.setItem('scrollPosition', window.scrollY);
};

// Restore the scroll position after the page loads
window.onload = function () {
    var scrollPosition = localStorage.getItem('scrollPosition');
    if (scrollPosition) {
        window.scrollTo(0, parseInt(scrollPosition));
        localStorage.removeItem('scrollPosition');
    }
};

document.addEventListener('DOMContentLoaded', function () {
    // Attach event listeners to all collapse triggers
    var collapseTriggers = document.querySelectorAll('[data-bs-toggle="collapse"]');
    console.log('Collapse triggers found:', collapseTriggers.length);

    collapseTriggers.forEach(function (trigger) {
        var targetId = trigger.getAttribute('href');
        console.log('Target ID:', targetId);

        if (targetId) {
            var targetContent = document.querySelector(targetId);
            console.log('Target content:', targetContent);

            if (targetContent) {
                var icon = trigger.querySelector('i');
                console.log('Icon:', icon);

                if (icon) {
                    // Bootstrap collapse events
                    targetContent.addEventListener('show.bs.collapse', function () {
                        icon.classList.remove('bi-chevron-down');
                        icon.classList.add('bi-chevron-up');
                    });

                    targetContent.addEventListener('hide.bs.collapse', function () {
                        icon.classList.remove('bi-chevron-up');
                        icon.classList.add('bi-chevron-down');
                    });
                } else {
                    console.error('Chevron icon not found inside the trigger.');
                }
            } else {
                console.error('Target content not found:', targetId);
            }
        } else {
            console.error('href attribute is missing or invalid.');
        }
    });
});
