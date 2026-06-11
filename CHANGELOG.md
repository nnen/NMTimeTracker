# Changelog

## v0.6

### New features
- **Modifier date range** — the Add Modifier dialog now has a "To" date field; setting it creates one modifier per day across the entire range.
- **Interval continuation** — if tracking is restarted within a configurable time window after stopping, the previous interval is resumed instead of creating a new one. Configurable in Settings ("Continuation window").
- **Running indicator icon** — the tray icon and taskbar icon switch to a green-handed clock while the timer is running.

### Improvements
- Settings window: added "Copy path" and "Show in Explorer" buttons next to the settings file and database file paths.

### Bug fixes
- Fixed double-clicking the tray icon not restoring the main window when it was minimised.

---

## v0.5

### New features
- **Day timeline control** — the history window now shows a colour-coded horizontal bar of all work intervals for the selected day, with selectable intervals, hour/half-hour tick marks, and a configurable default visible time range (6 AM – 8 PM).
- **History date range** — the history window has "From" / "To" date pickers that display the total worked and expected time across any date range.
- **Tray icon context menu** — right-clicking the tray icon exposes Pause/Resume, Add Modifier, Show History, and Exit actions.
- **Calendar non-work day highlighting** — weekend and zero-expected-time days are shown with a grey background in the history calendar.

### Improvements
- Modifier indicators are now shown in the day list.
- Expected time modifiers ("Holiday", "Vacation") now adjust the day's expected hours rather than the worked hours; a new "Paid Vacation" preset adds worked time.
- Month total and expected time are now displayed in business days (e.g. "2d 4h") instead of raw hours.

---

## v0.4

### New features
- **Always on top** — main window can be pinned above all other windows via the context menu.
- **Screensaver auto-pause** — timer is automatically paused when the screensaver activates.
- **Month total** — the history sidebar shows the total tracked time for the selected month.
- **Single-instance focus** — launching a second instance brings the existing window to the foreground instead of opening a duplicate.

### Improvements
- Settings window redesigned.
- First day of week is now read from the system locale rather than a separate setting.
- Interval start/stop reasons are displayed as human-readable strings; an in-progress interval is shown as "Still running" instead of "Unexpected exit".
- Version number, settings file path, and database file path are shown in the settings window.

### Bug fixes
- Fixed total time not updating correctly when resuming tracking the following day.
- Fixed a bug where more than one application instance could be launched simultaneously.

---

## v0.3

### New features
- **Modifier editing** — existing modifiers can be edited via a double-click or context menu in the history window.
- **Button icons** — the main window toolbar now shows icons on the Start, Stop, and Modifier buttons.
- **History button** — a dedicated History button was added to the main window.
- **Edit modifier context menu** — right-clicking a modifier in the history window shows Edit / Add / Delete options.

### Bug fixes
- Fixed time spans longer than 24 hours being displayed incorrectly.
