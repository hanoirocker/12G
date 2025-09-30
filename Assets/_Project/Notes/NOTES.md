Últimos cambios:
- Se creó un sistema independiente para examinar objetos (ExaminableObject.cs, ExaminationCanvasHandler, ExaminationTextSO, ObjectExaminationHandler);

TOFIX - urgente:
- Corregir método OnDrag en ExaminableObject.cs para guardar información de última rotación al presionar click nuevamente y rotar. Actualmente produce una reubicación extraña.

TODO - despues:
- Al comenzar un nuevo evento se ejecuta un parpadeo de pantalla. Por ahora sirve para seguir track de los mismos, pero hay que sacarlo eventualmente.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: