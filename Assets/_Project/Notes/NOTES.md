Últimos cambios:
- Se reemplazó el uso de TogglePlayerCapsule por EnablePlayerControllers. Esto permite desactivar todos los scripts que permiten interactuar con el jugador SIN apagar el game object player capsule. De esta forma se puede acceder al mismo para cambiar su transform durante cut scenes o cualquier otro momento en el que el jugador no está disponible.
- Se agregó un cut scene para el suicidio de fernandez, además de un nuevo modelo y un video para su televisor.

TOFIX:
- Los textos de textos internos desaparecen rápido si se pisan uno con el otro.


TODO:
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.
- Implementar cambios de Video del Settings Menu
- Mejorar todo el evento del suicidio. Esto incluye mejoras en el cut scene del mismo, agregar sangre en toda la escena y diálogos de Simon, audio (musica y resolver fuentes de ser usadas).

TOCHECK: