Últimos cambios:
- Se creó un sistema independiente para examinar objetos (ExaminableObject.cs, ExaminationCanvasHandler, ExaminationTextSO, ObjectExaminationHandler);

TOFIX - urgente:
- Corregir método OnDrag en ExaminableObject.cs para guardar información de última rotación al presionar click nuevamente y rotar. Actualmente produce una reubicación extraña.

TODO - próximo:
- Hacer examinable el cuadro de casamiento en la pieza de la hermana.
- Agregar revistas referidas a Travel & Fun con textos contemplables repetidos y únicos. También agregar al texto contemplable de algún cuadro de viaje la referencia sobre que Recife es el destino preferido de la madre.
- Agregar libros similares en estantería de zoom y hacerlos contemplables.
- Sebe disparar un evento "onSafeNoteExamined" que recibido por todos los prefabs involucrados y se activen componentes correspondientes de contemplación u examinación.

TODO - despues:
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: