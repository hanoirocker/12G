Últimos cambios:
- Se creó un sistema independiente para examinar objetos (ExaminableObject.cs, ExaminationCanvasHandler, ExaminationTextSO, ObjectExaminationHandler);
- Se agregó Old Radio y se dispara luego de examinar el Parents married portrait.
- Se recibe evento luego de revisar Saga Book 4 en el living, pero no se creo aún el prefab del trigger para las consecuencias del mismo.

TOFIX - urgente:
- Grabar sonidos para interacciones con cuadros, cajones, puerta de armario, botón de encendido/apagado de Old Radio.
- Algunos objetos al ser examinados atraviesan objetos cercanos. Buscar correción por proyección en otra capa?

TODO - próximo:
- Agregar revistas referidas a Travel & Fun con textos contemplables repetidos y únicos. También agregar al texto contemplable de algún cuadro de viaje la referencia sobre que Recife es el destino preferido de la madre.
- Agregar libros similares en estantería de zoom y hacerlos contemplables.

TODO - despues:
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: