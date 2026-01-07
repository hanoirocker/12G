- Se implementó el nuevo método "SetUpPlayerTransform" en EventsHandler.cs para colocar al jugador donde debe estar según el evento o tipo de juego (freeRoam o loadSpecificEvent).

TOCHECK:

TOFIX:
- El último "beep" del walkie talkie al terminar la conversación si el último dialogo fue de Simon, no se ejecuta.

- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales. // Esto sucede cuando no existe extructura de audios de canales para el evento en particular //

TODO - próximo:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún agregar lo que sucedería entre el final de Noises y el comienzo de Headaches donde choca el auto de la policia. Simon debería intentar contactarse con Mica luego de haber escuchado la conversación policial, pero sin lograrlo. Se podría mencionar el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Agregar sonidos stereo en loop para ambientación de cada habitación dentro de la casa. Se pueden disparar gracias al House Areas handler creado recientemente. 
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).

TODO - despues:

- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC
- Aumentar ganancia de tracks 12G - Door - Close (Hard) 2 y 12G - Door - Close (Hard) 1 ??
- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

