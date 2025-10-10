Últimos cambios:
- Se creó un sistema independiente para examinar objetos (ExaminableObject.cs, ExaminationCanvasHandler, ExaminationTextSO, ObjectExaminationHandler);
- Se agregó Old Radio y se dispara luego de examinar el Parents married portrait.
- Se recibe evento luego de revisar Saga Book 4 en el living, pero no se creo aún el prefab del trigger para las consecuencias del mismo.
- Se creó un nuevo asset para el renderer de URP "URP-HighFidelity-Renderer - RenderObjects Override" el cual es asignado al rendering del asset "URP-HighFidelity". El mismo tiene como objetivo evitar el clipping entre objetos FPS(inventario) o examinables.
- Se integro el sistema de examinacion con el inventario para que cuando el jugador examine objetos se oculte el walkie talkie y si esta usando una linterna se ilumine frontalmente el objeto.

TOFIX - urgente:
- Grabar sonidos para cuando come la pizza, toma un plato para servir la pizza, deposita la basura en el tacho, abre y cierra el tacho de basura, abre y cierra el armario.

TODO - próximo:

TODO - despues:
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: