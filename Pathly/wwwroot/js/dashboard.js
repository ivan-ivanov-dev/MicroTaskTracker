const DashboardUI = {
    init: function () {
        this.setGreeting();
        this.animateBars();
    },

    setGreeting: function () {
        const hour = new Date().getHours();
        const greetingEl = document.getElementById('dashboard-greeting');
        if (!greetingEl) return;

        let message = "Your journey is 1% better every day.";
        if (hour < 12) message = "🌞 Good morning! Let's start the day strong.";
        else if (hour < 18) message = "🚀 Afternoon push! You're doing great.";
        else message = "🌙 Great work today. Reflect and rest.";

        greetingEl.innerText = message;
    },

    animateBars: function () {
        // Triggered by CSS transition, but we can add JS logic here 
        // if we ever want to fetch real-time updates.
    }
};

document.addEventListener("DOMContentLoaded", () => DashboardUI.init());