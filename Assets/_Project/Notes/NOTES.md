Últimos cambios:
- Se agregó EnablePauseMenuCanvasAccess para permitir o no al jugador acceder al menu de pausa presionando la tecla escape.

TOFIX - urgente:
- El walkie talkie se puede agarrar.
- Necesitamos pensar en otra forma de alternar las VC's que no sean teniendolas instanciadas dentro de un solo prefab VC. Es mas cómodo si las VC's pertenecen al objeto con el cual el jugador interactua.

TODO - despues:
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mejorar todo el evento del suicidio. Esto incluye mejoras en el cut scene del mismo, agregar sangre en toda la escena y diálogos de Simon, audio (musica y resolver fuentes de ser usadas).
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK:
- Se puede abrir o cerrar la Safe Box desde afuera del ropero cuando sus puertas están cerradas?