Últimos cambios:
- Se creo el nuevo asset "ContextualContemplationSO" que permite integrar los ContemplationTextSO en rangos de eventos dentro de escenas especificas. De esta forma, al contemplar un objeto se puede mostrar textos distintos dependiendo del avance del juego. Para usarlo, se deben crear nuevos ContemplationTextSO para cada objeto para cada nueva instancia del juego, y agregarlos al ContextualContemplationSO correspondiente. Luego, este ContextualContemplationSO se referencia al ContemplableObject del objeto a contemplar.
- Se añadió un sistema de dialogos que permite incorporar texto en distintos lenguages y audio para sus respectivos clips en español. Trabajan por nodos, conectando un asset con otro o con su posible respuesta.

TOFIX - urgente:
- Grabar sonidos para cuando come la pizza, toma un plato para servir la pizza, deposita la basura en el tacho, abre y cierra el tacho de basura, abre y cierra el armario.
- AUDIO: eliminar audio sources en objetos interactuables (unicamente) y pedir al poolhandler en cambio que retorne una source libre para reproducir el clip y eventualmente asignar el transform del objeto interactuable. (EN PROCESO - CASI LISTO)

TODO - próximo:

TODO - despues:
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: