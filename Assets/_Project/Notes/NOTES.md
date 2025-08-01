Últimos cambios:
- Se cargan correctamente las escenas y eventos desde Nuevo Juego o Continuar
- Los eventos se actualizan y guardan correctamente
- El Pause Menu Canvas fue modificado.

TOFIX:


TODO:
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.
- Implementar cambios de Video del Settings Menu

TOCHECK:

- Actualmente si se continua una partida anterior, se carga la escena y se ejecuta desde el evento guardado PERO .. los objetos alterados en eventos anteriores no aparecen en las nuevas ubicaciones. Vale la pena guarda la información de todos estos objetos y setearlos al iniciar la partida guardada? O lo mejor es guardar únicamente por escenas? Quizas rompiendo las escenas en otras escenas mas chicas a modo checkpoints mas cercanos con el Environment y sus objetos modificados ... 