(() => {
  "use strict";

  const THEME_KEY = "realmaze-web-theme-v2";
  const root = document.documentElement;
  const toggle = document.getElementById("theme-toggle");

  function readTheme() {
    try {
      return localStorage.getItem(THEME_KEY) === "light"
        ? "light"
        : "dark";
    } catch (_) {
      return "dark";
    }
  }

  function applyTheme(theme, persist = true) {
    const normalized = theme === "light" ? "light" : "dark";
    root.dataset.theme = normalized;

    if (toggle) {
      const light = normalized === "light";
      toggle.textContent = light ? "Dark mode" : "Light mode";
      toggle.setAttribute("aria-pressed", String(light));
      toggle.setAttribute(
        "aria-label",
        light ? "Switch to dark mode" : "Switch to light mode"
      );
    }

    if (persist) {
      try {
        localStorage.setItem(THEME_KEY, normalized);
      } catch (_) {}
    }
  }

  applyTheme(readTheme(), false);

  if (toggle) {
    toggle.addEventListener("click", () => {
      applyTheme(root.dataset.theme === "light" ? "dark" : "light");
    });
  }
})();
