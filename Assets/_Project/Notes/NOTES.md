- Se ha agregado textos al canvas de Examination.


TOFIX:
- DizzinessHandler .. al rotar el jugador mientras se aplica el efecto se exprimentan tirones de giro.

TODO - próximo:
- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Agregar cajas bajo la escalera, o algo.
- Continuar con evento Headaches: hasta ahora solo hay choque de policia.
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Electric Feel FX: alterar entre intesidad máxima y mín dependiendo de si el jugador acerca o no el WT a su cara?

TODO - despues:
- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- SISTEMA GUARDADO POR CHECKPOINTS: Dividir las escenas evening y night en escenas mas chicas con menos eventos, y configurar cada escena con los objetos necesarios para usarlas como punto de restauracion de juego.
- Posiblidad de muerte por sobreexposición a la resonancia?
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK:
- aumentar umbral de mareos?