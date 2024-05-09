2d platformer game Created based on Tutorial video of "Gold Metal" on Youtube.

While making this by following the tutorial, I learned basics of how Unity work.

Also, I added some of my own unique functions, such as time limit per stages, background music, etc.

--------------------------------------------------
**Functions added**
--------------------------------------------------
Using raycasthit2d, player character checks if the character is colliding with floor/wall.
Player can jump, and jump count resets when the character lands on the floor. In this game, max. number of jump available at once is two(double jump), but this can be easily changed by changing an int var.
When character is colliding with wall, player can try wall jump using jump key(Space bar).
