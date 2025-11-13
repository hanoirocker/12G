- Se trabajo en el sistema del walkie talkie y del eventhandler. Ahora el EventHandler avisa a traves de un evento cada vez que comienza un
evento nuevo enviando info de la SceneEnum y EventEnum. Esto es atrapado por ejemplo por el Walkie Talkie que usara esta info para actualizar su base de datos de audios por escena y evento. Estas bases de datos son las WalkieTalkieDataSO y existiran una por evento.

TOFIX - urgente:
- Grabar sonidos para cuando come la pizza, toma un plato para servir la pizza, deposita la basura en el tacho, abre y cierra el tacho de basura, abre y cierra el armario.
- AUDIO: eliminar audio sources en objetos interactuables (unicamente) y pedir al poolhandler en cambio que retorne una source libre para reproducir el clip y eventualmente asignar el transform del objeto interactuable. (EN PROCESO - CASI LISTO)

TODO - próximo:
- Mover el canvas de diálogos hacia una posición por debajo. Quizas desactivar contemplación y observación (pero no el CameraZoom) durante un proceso de diálogo?

TODO - despues:
- Crear un objeto de post procesing por cada escena con parámetros específicos y otro común que compartan todas.
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: