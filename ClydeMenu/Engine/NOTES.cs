// [Noise Logger]
// Display nearby sounds as subtitles in the bottom-right corner of the screen.
// Each subtitle includes:
// - A short label (e.g. "Footsteps", "Robe Screams")
// - A horizontal arrow indicating direction relative to the player's forward vector:
//   - ← if sound is to the left
//   - → if sound is to the right
// Only log sounds actually heard by the local player (within audible range and volume threshold).
// Subtitles fade out after a short duration (e.g. 3–5 seconds).

// [Missed Room Alerts]
// For each room, track whether the local player has entered it this round.
// Unexplored rooms are outlined in orange (on the map or HUD).

// [Team Tracker]
// On hold — tracking the value of loot held(or destroyed) by teammates.
// Not sure about this one

// [Basic Macros]
// On hold - to automate stuff like tumble jumping higher then usual
// this could be seen as unfair

// [Visual Indicators]
// Packets - Display ping packet loss and other misc things that are higher then normal
// Helper - subtle screen effects for if a player drops an item nearby (and loses value)
// Helper - arrows on the side of the screen to indicate a player is nearby (or a cart)
// Helper - this could also loop back around to subtitles for sounds ^
// On hold - drawing a trajectory line for thrown items
// not sure if this would even be useful

// [Map Info]
// display map info on the screen (for example shows how many people are still alive if you have the upgrade for it)
// Waypoints - placing waypoints on the map to mark important locations with colours & 4 letter tags
//    - could put it on top of the screen on a bar similar to Rust's compass

// [Breadcrumbs]
// On hold - drawing breadcrumb trails where players and carts go
// might be to much

// [Keystrokes]
// Everyone loves a good old keystrokes module

// [Freelook]
// Hold alt to freeze the camera direction and rotate it freely without effecting the movement vectors
